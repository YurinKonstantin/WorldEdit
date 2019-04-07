using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WorldEdit
{
   public class VMV
    {
        int countStroc = 0;
        public int CountStroc
        {
            get
            {
                return countStroc;
            }
            set
            {
                countStroc = value;
            }
        }
        public string CountStrocFor
        {
            get
            {
                return "Строк: " + CountStroc.ToString();
            }
        }
        int countStrocSelect = 0;
        public int CountStrocSelect
        {
            get
            {
                return countStrocSelect;
            }
            set
            {
                countStrocSelect = value;
            }
        }
        public string CountStrocSelectFor
        {
            get
            {
                return "Строк: " + countStrocSelect.ToString();
            }
        }
        StorageFile file;
        public StorageFile File
        {
            get
            {
                return file;
            }
            set
            {
                file = value;
            }
        }
    }
}
