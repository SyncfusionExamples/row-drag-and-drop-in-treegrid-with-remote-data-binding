using Microsoft.AspNetCore.Mvc;
using RowDD.Models;
using System.Diagnostics;
using System.Collections;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Linq;

namespace RowDD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult UrlDatasource([FromBody] DataManagerRequest dm)
        {
            IEnumerable DataSource = TreeGridItems.GetSelfData();
            DataOperations operation = new DataOperations();

            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<TreeGridItems>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Ok(new { result = DataSource, count }) : Ok(DataSource);

        }
        public IEnumerable CollectChildRecords(IEnumerable datasource, DataManagerRequest dm)
        {
            DataOperations operation = new DataOperations();
            IEnumerable DataSource = TreeGridItems.GetSelfData(); // use the total DataSource here
            string IdMapping = "TaskId";// define your IdMapping field name here
            int[] Ids = new int[0];
            foreach (var rec in datasource)
            {
                int ID = (int)rec.GetType().GetProperty(IdMapping).GetValue(rec);
                Ids = Ids.Concat(new int[] { ID }).ToArray();
            }
            IEnumerable ChildRecords = null;
            foreach (int id in Ids)
            {
                dm.Where[0].value = id;
                IEnumerable records = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                ChildRecords = ChildRecords == null || (ChildRecords.AsQueryable().Count() == 0) ? records :
                 ((IEnumerable<object>)ChildRecords).Concat((IEnumerable<object>)records);
            }
            if (ChildRecords != null)
            {
                ChildRecords = CollectChildRecords(ChildRecords, dm);
                if (dm.Sorted != null && dm.Sorted.Count > 0)
                {
                    ChildRecords = operation.PerformSorting(ChildRecords, dm.Sorted);
                }
                datasource = ((IEnumerable<object>)datasource).Concat((IEnumerable<object>)ChildRecords);
            }
            return datasource;
        }

        public bool MyTestMethod([FromBody] ICRUDModel value)
        {
            if (value.pos.position == "bottomSegment" || value.pos.position == "topSegment")
            {
                //for bottom and top segment drop position. If the dragged record is the only child for a particular record,
                //we need to set parentItem of dragged record to null and isParent of dragged record's parent to false
                if (value.value.ParentId != null) // if dragged record has parent
                {
                    var childCount = 0;
                    int parent1 = (int)value.value.ParentId;
                    childCount += FindChildRecords(parent1); // finding the number of child for dragged record's parent
                    if (childCount == 1) // if the dragged record is the only child for a particular record
                    {
                        var i = 0;
                        for (; i < TreeGridItems.GetSelfData().Count; i++)
                        {
                            if (TreeGridItems.GetSelfData()[i].TaskId == parent1)
                            {
                                //set isParent of dragged record's parent to false
                                TreeGridItems.GetSelfData()[i].isParent = false;
                                break;
                            }
                            if (TreeGridItems.GetSelfData()[i].TaskId == value.value.TaskId)
                            {
                                //set parentItem of dragged record to null
                                TreeGridItems.GetSelfData()[i].ParentId = null;
                                break;
                            }


                        }
                    }
                }
                TreeGridItems.GetSelfData().Remove(TreeGridItems.GetSelfData().Where(ds => ds.TaskId == value.pos.dragidMapping).FirstOrDefault());
                var j = 0;
                for (; j < TreeGridItems.GetSelfData().Count; j++)
                {
                    if (TreeGridItems.GetSelfData()[j].TaskId == value.pos.dropidMapping)
                    {
                        //set drgged records parentItem with parentItem of
                        //record in dropindex
                        value.value.ParentId = TreeGridItems.GetSelfData()[j].ParentId;
                        break;
                    }
                }
                if (value.pos.position == "bottomSegment")
                {
                    this.Insert(value, value.pos.dropidMapping);
                }
                else if (value.pos.position == "topSegment")
                {
                    //TreeGridItems.GetSelfData().Remove(TreeGridItems.GetSelfData().Where(ds => ds.TaskId == pos.dragidMapping).FirstOrDefault());
                    this.InsertAtTop(value, value.pos.dropidMapping);
                }

            }
            else if (value.pos.position == "middleSegment")
            {
                TreeGridItems.GetSelfData().Remove(TreeGridItems.GetSelfData().Where(ds => ds.TaskId == value.pos.dragidMapping).FirstOrDefault());
                value.value.ParentId = value.pos.dropidMapping;
                FindDropdata(value.pos.dropidMapping);
                this.Insert(value, value.pos.dropidMapping);
            }
            return true;
        }

        public ActionResult Update([FromBody] ICRUDModel value)
        {
            if (value != null)
            {
                var val = TreeGridItems.GetSelfData().Where(ds => ds.TaskId == value.value.TaskId).FirstOrDefault();
                val.TaskName = value.value.TaskName;
                val.Duration = value.value.Duration;
                return Json(val);
            }
            else return Json(null);

        }

        public ActionResult Insert([FromBody] ICRUDModel value, int rowIndex)
        {
            var i = 0;
            if (value.Action == "insert")
            {
                rowIndex = value.relationalKey;
            }
            Random ran = new Random();




            int a = ran.Next(100, 1000);
            //value.value.TaskId = a;
            for (; i < TreeGridItems.GetSelfData().Count; i++)
            {
                if (TreeGridItems.GetSelfData()[i].TaskId == rowIndex)
                {
                    if (value.pos.position == "middleSegment")
                    {
                        value.value.ParentId = rowIndex;
                        if (TreeGridItems.GetSelfData()[i].isParent == false)
                        {
                            TreeGridItems.GetSelfData()[i].isParent = true;
                        }
                    }
                    break;

                }
            }
            i += FindChildRecords(rowIndex);
            TreeGridItems.GetSelfData().Insert(i, value.value);

            return Json(value.value);
        }

        public void InsertAtTop([FromBody] ICRUDModel value, int rowIndex)
        {
            var i = 0;
            for (; i < TreeGridItems.GetSelfData().Count; i++)
            {
                if (TreeGridItems.GetSelfData()[i].TaskId == rowIndex)
                {
                    break;

                }
            }
            // i += FindChildRecords(rowIndex);
            TreeGridItems.GetSelfData().Insert(i, value.value);
        }

        public void FindDropdata(int key)
        {
            var i = 0;
            for (; i < TreeGridItems.GetSelfData().Count; i++)
            {
                if (TreeGridItems.GetSelfData()[i].TaskId == key)
                {
                    TreeGridItems.GetSelfData()[i].isParent = true;
                }
            }
        }

        public int FindChildRecords(int? id)
        {
            var count = 0;
            for (var i = 0; i < TreeGridItems.GetSelfData().Count; i++)
            {
                if (TreeGridItems.GetSelfData()[i].ParentId == id)
                {
                    count++;
                    count += FindChildRecords(TreeGridItems.GetSelfData()[i].TaskId);
                }
            }
            return count;
        }
        public void Remove([FromBody] ICRUDModel value)
        {
            if (value.Key != null)
            {
                // TreeGridItems value = key;
                TreeGridItems.GetSelfData().Remove(TreeGridItems.GetSelfData().Where(ds => ds.TaskId == double.Parse(value.Key.ToString())).FirstOrDefault());
            }

        }

        public object Delete([FromBody] ICRUDModel value)
        {

            if (value.Deleted != null)
            {
                for (var i = 0; i < value.Deleted.Count; i++)
                {
                    TreeGridItems.GetSelfData().Remove(TreeGridItems.GetSelfData().Where(ds => ds.TaskId == value.Deleted[i].TaskId).FirstOrDefault());
                }
            }
            return new { deleted = value.Deleted };
        }

        public class CustomBind : TreeGridItems
        {
            public TreeGridItems ParentId;
        }

        public class ICRUDModel
        {
            public TreeGridItems value;

            public TreeGridData pos;
            public int relationalKey { get; set; }
            public string Action { get; set; }

            public string Table { get; set; }

            public string KeyColumn { get; set; }

            public List<TreeGridItems> Added { get; set; }

            public List<TreeGridItems> Changed { get; set; }

            public List<TreeGridItems> Deleted { get; set; }
            public object Key { get; set; }
            public IDictionary<string, object> Params { get; set; }
        }
        public class TreeGridData
        {
            public int dragidMapping { get; set; }
            public int dropidMapping { get; set; }
            public string position { get; set; }
        }
        public class TreeClass : TreeGridItems
        {
            public TreeGridItems taskData { get; set; }
        }
    }
    public class TreeGridItems
    {
        public TreeGridItems() { }
        public int TaskId { get; set; }



        public string TaskName { get; set; }
        public int Duration { get; set; }
        public int? ParentId { get; set; }
        public bool? isParent { get; set; }


        public static List<TreeGridItems> BusinessObjectCollection = new List<TreeGridItems>();



        public static List<TreeGridItems> GetSelfData()
        {
            if (BusinessObjectCollection.Count == 0)
            {
                int numberOfParentTasks = 20; // Define how many parent tasks you want
                int taskId = 1; // Starting TaskId

                for (int i = 0; i < numberOfParentTasks; i++)
                {
                    // Add a parent task
                    BusinessObjectCollection.Add(new TreeGridItems()
                    {
                        TaskId = taskId++,
                        TaskName = $"Parent Task {i + 1}",
                        Duration = 10,
                        ParentId = null,
                        isParent = true
                    });

                    int parentId = taskId - 1; // Current parent task ID

                    // Add 5 child tasks for each parent task
                    for (int j = 0; j < 5; j++)
                    {
                        BusinessObjectCollection.Add(new TreeGridItems()
                        {
                            TaskId = taskId++,
                            TaskName = $"Child Task {i + 1}-{j + 1}",
                            Duration = 4,
                            ParentId = parentId,
                            isParent = false
                        });
                    }
                }
            }
            return BusinessObjectCollection;
        }
    }
}
