using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
   public class CNode
    {

        readonly List<CNode> _children = new List<CNode>();

       
        public CNode(DataRow datarow, DataTable dt)
        {
            string id=datarow[0].ToString();

            this.Name = datarow[1].ToString();
            this.ID = id;
            this.Content = Convert.ToBoolean(datarow[3]);

            DataRow[] dr = dt.Select($"Parent_id= '{id}'");            
            foreach (DataRow ch in dr)
            {
                CNode n =new CNode(ch, dt);
                _children.Add(n);
            }
        }

              
             
        public IList<CNode> Children
        {
            get { return _children; }
            
        }

        public string Name { get; set; }
        public string ID { get; set; }

        public Boolean Content { get; set; }
    }
}
