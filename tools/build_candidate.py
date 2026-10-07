"""Create preliminary Unity-format archives without launching Unity."""
from pathlib import Path
import gzip,hashlib,io,json,re,tarfile
from validate_package import validate

project=Path(__file__).resolve().parents[1];package=project/'Assets/ModularGridInventory';output=project/'Builds/Package';output.mkdir(parents=True,exist_ok=True)
report=validate(project,package)
if report['errors']:raise SystemExit('\n'.join(report['errors']))
artifacts=[]
for addon in [False,True]:
    name='ModularGridInventory'+('-InputSystem' if addon else '')+'-0.1.0-candidate.unitypackage';target=output/name
    paths=[package]
    for path in sorted(package.rglob('*')):
        if path.suffix=='.meta':continue
        in_addon='Integrations' in path.relative_to(package).parts
        if in_addon==addon:paths.append(path)
    with target.open('wb') as stream,gzip.GzipFile(filename='',mode='wb',fileobj=stream,mtime=0) as compressed,tarfile.open(fileobj=compressed,mode='w') as archive:
        for path in paths:
            meta=Path(str(path)+'.meta');guid=re.search(r'^guid: ([a-f0-9]{32})$',meta.read_text(),re.M)[1]
            data={'pathname':path.relative_to(project).as_posix().encode(),'asset.meta':meta.read_bytes()}
            if path.is_file():data['asset']=path.read_bytes()
            for filename,content in data.items():
                entry=tarfile.TarInfo(guid+'/'+filename);entry.size=len(content);entry.mtime=0;entry.mode=0o644;archive.addfile(entry,io.BytesIO(content))
    with tarfile.open(target,'r:gz') as archive:
        members=archive.getmembers();pathnames=[archive.extractfile(m).read().decode() for m in members if m.name.endswith('/pathname')]
        if len(pathnames)!=len(paths) or any(not p.startswith('Assets/ModularGridInventory') for p in pathnames):raise RuntimeError('Archive content mismatch')
    artifacts.append({'file':name,'bytes':target.stat().st_size,'sha256':hashlib.sha256(target.read_bytes()).hexdigest(),'assets':len(paths),'unityImportVerified':False})
(output/'candidate.json').write_text(json.dumps({'version':'0.1.0-candidate','artifacts':artifacts,'validation':report['counts'],'unityImportVerified':False},indent=2)+'\n',encoding='utf-8')
print(json.dumps(artifacts,indent=2))
