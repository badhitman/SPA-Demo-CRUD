////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import { aWarehouseDocumentCreating } from '../aWarehouseDocumentCreating';

/** Создание новго документа поступления на склад */
export class createReceiptWarehouse extends aWarehouseDocumentCreating {
    static displayName = createReceiptWarehouse.name;
    modelName = 'ReceiptToWarehouseDocumentModel';
    userFriendlyNameDocument = 'Поступление на склад';
}
