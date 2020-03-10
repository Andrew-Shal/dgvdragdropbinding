using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datagridviewtemplate.Dependencies
{

    public class Paginator
    {
        private int _PageSize;
        private int _CurrentPageIndex;
        private int _TotalPage;
        public int PageSize { get => _PageSize; set => _PageSize = value; }
        public int CurrentPageIndex { get => _CurrentPageIndex; set => _CurrentPageIndex = value; }
        public int TotalPage { get => _TotalPage; set => _TotalPage = value; }

        public Paginator()
        {
            this._PageSize = 3;
            this._CurrentPageIndex = 1;
            this._TotalPage = 0;
        }
        public Paginator(int pgSize, int currentPageIndex, int totalPage)
        {
            this._PageSize = pgSize;
            this._CurrentPageIndex = currentPageIndex;
            this._TotalPage = totalPage;
        }


        public void CalculateTotalPages(int totalRecords)
        {
            int rowCount = totalRecords;
            this._TotalPage = rowCount / this._PageSize;
            // if any row left after calculated pages, add one more page 
            if (rowCount % this._PageSize > 0)
            {
                this._TotalPage += 1;
            }
        }
    }
}

