/** Модель для передачи параметров для формирования кнопки */
export class PaginationNavElement {
    /**
     * Конструктор кнопки пагинатора
     * @param {PaginationTypesButton} typeButton - тип кнопки: Назад, Номер страницы, Разделитель, Далее
     * @param {boolean} isActive - признак того что кнопка активная
     * @param {string} title - текст кнопки
     * @param {string} href - ссылка кнопки
     */
    constructor(typeButton, isActive, title, href) {
        /** Тип кнопки: Назад, Номер страницы, Разделитель, Далее */
        this.typeButton = typeButton;

        /** Признак того что кнопка активная */
        this.isActive = isActive;

        /** Текст кнопки */
        this.title = title;

        /** Ссылка кнопки */
        this.href = href;
    }
}
