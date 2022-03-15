using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models.TreeStruct;

namespace WpfApp1.ViewModels.TreeStructPages
{
    class TextsFileViewModel : IPageViewModel
    {
        public string Name
        {
            get { return "Файл описаний"; }
        }

        public void Dispose() { }

        public void DeleteItem(int id)
        {
            List<TextsStruct> indexes = new List<TextsStruct>();
            //List<PicturesStruct> list = _picturesFileItems.ToList().FindAll(i => i.TreeId == id);
            foreach (TextsStruct item in _textsFileItems)
            {
                if (item.DR.RowState == DataRowState.Detached)
                {
                    indexes.Add(item);
                }
            }
            foreach (TextsStruct ind in indexes)
            {
                _textsFileItems.Remove(ind);
            }
            indexes = null;
        }

        public TextsFileViewModel(Database db)
        {
            _textsFileItems = new List<TextsStruct>();
            foreach (DataRow dr in db.dataset1.Texts)
            {
                _textsFileItems.Add(new TextsStruct(dr));
            }
        }

        List<TextsStruct> _textsFileItems;

        public List<TextsStruct> TextsFileItems
        {
            get
            {
                return _textsFileItems;
            }
        }
    }
}
