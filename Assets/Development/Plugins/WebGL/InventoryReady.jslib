mergeInto(LibraryManager.library, {
  InventoryReady: function () {
    window.dispatchEvent(new Event("inventory-ready"));
  }
});
