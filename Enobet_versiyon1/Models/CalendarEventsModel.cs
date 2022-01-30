using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class CalendarEventsModel
    {
        //public int ProjectId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string color{get;set;}
        public bool allDay { get; set; }
        public int MahalId { get; set; }
    //    public List<DegerlendirmeDurumu> DegerlendirmeDurumu { get; set; }
      //  public ProjectModel Proje { get; set; }

        public TaskAddOrUpdateModel TaskAddOrUpdate { get; set; }
        public CalendarEventsModel()
        {
           // DegerlendirmeDurumu = new List<DegerlendirmeDurumu>();
          //  Proje = new ProjectModel();

      /*      TaskAddOrUpdate.AtananKulllaniciIdList = new List<int>();
           TaskAddOrUpdate.AtananKulllaniciMailList = new List<string>();

            TaskAddOrUpdate.KullanicininArkadaslariIdList = new List<int>();
            TaskAddOrUpdate.KullanicininArkadaslariMailList = new List<string>();*/

        }

        public class ProjectModel
        {
            public int ProjectId { get; set; }
        }

        public class TaskAddOrUpdateModel
        {
            public string TaskDetail { get; set; }
            public List<int> AtananKulllaniciIdList { get; set; }
            public List<string> AtananKulllaniciMailList { get; set; }
            public List<int> KullanicininArkadaslariIdList { get; set; }
            public List<string> KullanicininArkadaslariMailList { get; set; }
        }

    }
}