var dataManager = new ej.data.DataManager({
    adaptor: new ej.data.UrlAdaptor,
    url: "http://localhost:5077/Home/UrlDatasource",
    insertUrl: "http://localhost:5077/Home/Insert",
    removeUrl: "http://localhost:5077/Home/Remove",
    updateUrl: "http://localhost:5077/Home/Update",
    crossDomain: true
});
var treegrid = new ej.treegrid.TreeGrid({
    dataSource: dataManager,
    idMapping: 'TaskId',
    hasChildMapping: 'isParent',
    parentIdMapping: 'ParentId',
    allowRowDragAndDrop: true,
    height: 400,
    rowHeight: 50,
    editSettings: {
        allowAdding: true,
        allowEditing: true,
        allowDeleting: true,
        mode: 'Cell',
        newRowPosition: 'Below'
    },
    toolbar: ['Add', 'Delete', 'Update', 'Cancel', 'Indent', 'Outdent'],
    treeColumnIndex: 1,
    columns: [
        { field: 'TaskId', headerText: 'Task ID', width: 90, textAlign: 'Right', isPrimaryKey: true },
        { field: 'TaskName', headerText: 'Task Name', width: 180, },
    ],
    rowDrop: function (args) {
        var tree = document.getElementsByClassName('e-treegrid')[0].ej2_instances[0];
        var proxy = tree;
        var record1 = tree.getCurrentViewRecords()[args.fromIndex][tree.idMapping];
        var record2 = tree.getCurrentViewRecords()[args.dropIndex][tree.idMapping];
        var data = args.data[0];
        var currentViewRecord = tree.getCurrentViewRecords();
        var primaryKeyList = [];
        for (var i = 0; i < currentViewRecord.length; i++) {
            if (currentViewRecord[i].isParent && !currentViewRecord[i].expanded) {
                primaryKeyList.push(currentViewRecord[i].TaskId);
            }
        }
        var position = { dragidMapping: record1, dropidMapping: record2, position: args.dropPosition };
        args.cancel = true;
        var fetch = new ej.base.Fetch("http://localhost:5077/Home/MyTestMethod", "POST");
        fetch.data = JSON.stringify({ value: data, pos: position });
        fetch.send();
        fetch.onSuccess = function (data) {
            proxy.refresh();
        };
    }
});
treegrid.appendTo('#TreeGrid');