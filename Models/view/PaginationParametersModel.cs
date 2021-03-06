////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace SPADemoCRUD.Models
{
    public class PaginationParametersModel
    {
        public readonly static int[] PaginationSizes = new int[] { 5, 10, 20, 50, 100, 200 };

        /// <summary>
        /// Инициализация/оптимизация состояния пагинатора
        /// </summary>
        /// <param name="itemsCount">количество элементов всего в коллекции</param>
        public void Init(int items_count)
        {
            itemsCount = items_count;
            pageSize = CheckPageSize(pageSize);
            pageNum = CheckPageNum(pageNum);
        }

        /// <summary>
        /// Перезагрузить состояние "пагинатора" для формирования постраничного вывода данных.
        /// ВНИМАНИЕ! Переданый 'List' будет усечён до "актуального состояния" в зависимости от запрошеного номера страницы и настроек размера страницы
        /// </summary>
        /// <param name="data_list">Многострочные данные для формирования постраничного документа. Переданый список будет "усечён до актуального состояния" в зависимости от запрошеного номера страницы и настроек размера страницы</param>
        public void Init<T>(ref List<T> data_list)
        {
            CountAllElements = data_list.Count;

            if (PageNum == 1)
                data_list = new List<T>(data_list.Take(PageSize));
            else
                data_list = new List<T>(data_list.Skip(Skip).Take(PageSize));
        }

        /// <summary>
        /// Количество страниц в постраничном документе
        /// </summary>
        public int CountPages
        {
            get
            {
                if (CountAllElements <= 0 || PageSize <= 0)
                    return 0;

                return Convert.ToInt16(Math.Ceiling((double)CountAllElements / (double)PageSize));
            }
        }

        /// <summary>
        /// Проверка корректности номера страницы
        /// </summary>
        /// <param name="page_num">номер страницы для проверки</param>
        /// <returns>Корректное значение номера страницы</returns>
        public int CheckPageNum(int page_num)
        {
            if (page_num <= 0)
                return 1;

            if (CountPages < page_num)
                page_num = CountPages;

            return page_num;
        }

        /// <summary>
        /// Проверка корректности размерности страницы (количество элементов на страницу)
        /// </summary>
        /// <param name="page_size">Размерность для проверки</param>
        /// <returns>Корректное значение размера страницы</returns>
        public int CheckPageSize(int page_size)
        {
            int min_pagesize = PaginationSizes.Min();
            int max_pagesize = PaginationSizes.Max();

            if (min_pagesize > page_size)
                return min_pagesize;

            if (max_pagesize < page_size)
                return max_pagesize;

            if (CountAllElements < page_size)
                page_size = CountAllElements;

            return page_size;
        }

        private int itemsCount = 0;
        public int CountAllElements { get => itemsCount; private set => itemsCount = value; }

        private int pageNum = 1;
        public int PageNum
        {
            get => pageNum;
            set => pageNum = value;
        }

        private int pageSize = 20;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = value;
        }

        /// <summary>
        /// Вычислить количество строк данных, которые нужно пропустить исходя из номера текущей страницы
        /// </summary>
        public int Skip { get { return (PageNum - 1) * PageSize; } }
    }
}
