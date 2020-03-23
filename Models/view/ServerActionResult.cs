////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models.view
{
    /// <summary>
    /// Результат выполнения запроса сервером
    /// </summary>
    public class ServerActionResult
    {
        /// <summary>
        /// статус операции (успешно или нет)
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// стиль оформления сообщения (bootstrap alert)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Дополнительные/технические данные ответа (в случае необходимости)
        /// </summary>
        public object Tag { get; set; }
    }
}
