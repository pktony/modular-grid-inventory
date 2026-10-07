"""Offline checks only; Unity import and execution must still be validated."""
from pathlib import Path
import argparse, hashlib, json, re
from PIL import Image
import yaml

def documents(path):
    text=path.read_text(encoding='utf-8-sig')
    text=re.sub(r'^%.*\n','',text,flags=re.M)
    text=re.sub(r'^--- !u!\d+ &-?\d+.*$', '---', text, flags=re.M)
    return list(yaml.safe_load_all(text))

def validate(project, package):
    errors=[]; entries=[]; guid_paths={}; package_guids=set()
    def require(condition,message):
        if not condition:errors.append(message)
    for meta in package.parent.rglob('*.meta'):
        if not (meta==Path(str(package)+'.meta') or package in meta.parents):continue
        match=re.search(r'^guid: ([a-f0-9]{32})$',meta.read_text(),re.M)
        require(match is not None,f'Missing GUID: {meta}')
        if not match:continue
        guid=match[1];path=Path(str(meta)[:-5])
        require(guid not in guid_paths,f'Duplicate GUID: {guid}')
        guid_paths[guid]=path;package_guids.add(guid)
    for meta in (project/'Assets').rglob('*.meta'):
        if meta==Path(str(package)+'.meta') or package in meta.parents:continue
        match=re.search(r'^guid: ([a-f0-9]{32})$',meta.read_text(encoding='utf-8-sig'),re.M)
        if match:require(match[1] not in package_guids,f'GUID collision with host asset: {meta.relative_to(project)}')
    for meta in (project/'Library/PackageCache').glob('com.unity.ugui*/**/*.meta'):
        match=re.search(r'^guid: ([a-f0-9]{32})$',meta.read_text(encoding='utf-8-sig'),re.M)
        if match:guid_paths.setdefault(match[1],Path(str(meta)[:-5]))
    for path in [package,*sorted(package.rglob('*'))]:
        if path.suffix=='.meta':continue
        relative=path.relative_to(project).as_posix()
        require(len(relative)<150,f'Path length: {relative}')
        require(Path(str(path)+'.meta').exists(),f'Missing metadata: {relative}')
        require(path.suffix.lower() not in ['.exe','.dll','.mp4','.zip'],f'Forbidden artifact: {relative}')
        if path.is_file():
            entries.append({'path':relative,'bytes':path.stat().st_size,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()})
            if path.suffix in ['.asset','.unity','.prefab']:
                for guid in re.findall(r'guid: ([a-f0-9]{32})',path.read_text(encoding='utf-8-sig')):
                    require(guid in guid_paths or guid.startswith('0000000000000000'),f'Unresolved reference {guid} in {relative}')
            if path.suffix in ['.cs','.asset','.json','.meta']:
                require(not re.search(r'tarkov|blackrock|wartech|flyye|aks-74u|zenit',path.read_text(encoding='utf-8-sig'),re.I),f'Brand reference: {relative}')
    assets={}
    for guid,path in guid_paths.items():
        if guid in package_guids and path.suffix=='.asset' and path.parent.name=='Catalog':
            assets[guid]=documents(path)[0]['MonoBehaviour']
    items={v['identifier']:v for v in assets.values() if 'maxStack' in v}
    require(len(items)==19,'Expected 19 item definitions')
    require(sum(1 for key in items if key=='rig' or key.startswith('rig-'))==10,'Expected 10 carrier definitions')
    state=documents(package/'Samples/Settings/InitialState.asset')[0]['MonoBehaviour']['entries']
    storage=documents(package/'Samples/Settings/RootContainer.asset')[0]['MonoBehaviour']
    require(len(state)==22,'Expected 22 sample instances')
    keys={entry['key']:entry for entry in state};require(len(keys)==len(state),'Duplicate seed keys')
    occupied={};parents={}
    for entry in state:
        item=assets.get(entry['item']['guid']);require(item is not None,f'Missing seed item: {entry["key"]}')
        if item is None:continue
        require(1<=entry['quantity']<=item['maxStack'],f'Invalid quantity: {entry["key"]}')
        parent=entry['parentKey'] or ''
        if parent:
            require(parent in keys,f'Missing parent: {entry["key"]}')
            if parent not in keys:continue
            parent_item=assets[keys[parent]['item']['guid']]
            container=assets.get(parent_item['container'].get('guid'));require(container is not None,f'Noncontainer parent: {entry["key"]}')
            if container is None:continue
        else:container=storage
        parents[entry['key']]=parent
        sections={s['identifier']:s for s in container['sections']};section=sections.get(entry['section'])
        require(section is not None,f'Missing pocket: {entry["key"]}')
        if section is None:continue
        width,height=(item['height'],item['width']) if entry['rotated'] else (item['width'],item['height'])
        require(entry['x']>=0 and entry['y']>=0 and entry['x']+width<=section['width'] and entry['y']+height<=section['height'],f'Out of bounds: {entry["key"]}')
        location=(parent,entry['section']);cells=occupied.setdefault(location,set())
        footprint={(x,y) for x in range(entry['x'],entry['x']+width) for y in range(entry['y'],entry['y']+height)}
        require(not cells.intersection(footprint),f'Overlapping seed: {entry["key"]}');cells.update(footprint)
    for key in parents:
        visited=set();cursor=key
        while cursor:
            require(cursor not in visited,f'Container cycle: {key}')
            if cursor in visited:break
            visited.add(cursor);cursor=parents.get(cursor,'')
    icons=list((package/'Samples/Icons').rglob('*.png'));require(len(icons)==19,'Expected 19 icons')
    records=json.loads((package/'Documentation/GeneratedAssets.json').read_text())['assets']
    for record in records:
        path=package/record['file'];require(hashlib.sha256(path.read_bytes()).hexdigest()==record['sha256'],f'Icon hash mismatch: {path.name}')
        with Image.open(path) as image:require(image.mode=='RGBA' and image.getextrema()[-1][0]==0,f'Icon transparency: {path.name}')
    names={json.loads(p.read_text())['name'] for p in package.rglob('*.asmdef')}
    for path in package.rglob('*.asmdef'):
        data=json.loads(path.read_text())
        for reference in data.get('references',[]):
            require(reference in names or reference in ['Unity.ugui','Unity.TextMeshPro','Unity.InputSystem'],f'Unexpected assembly reference: {reference}')
            if 'Runtime' in path.parts:require('Development' not in reference and not reference.endswith('.Samples') and not reference.endswith('.Editor'),f'Runtime boundary: {reference}')
    return {'status':'pass' if not errors else 'fail','errors':errors,'files':entries,'counts':{'files':len(entries),'icons':len(icons),'items':len(items),'seedInstances':len(state)},'unityImportVerified':False,'unityTestsExecuted':False}

if __name__=='__main__':
    parser=argparse.ArgumentParser();parser.add_argument('--project',type=Path,default=Path(__file__).resolve().parents[1]);parser.add_argument('--report',type=Path,default=Path('Builds/Package/validation.json'))
    args=parser.parse_args();project=args.project.resolve();report=validate(project,project/'Assets/ModularGridInventory')
    args.report.parent.mkdir(parents=True,exist_ok=True);args.report.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({key:value for key,value in report.items() if key!='files'},indent=2));raise SystemExit(bool(report['errors']))
