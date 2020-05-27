////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

import { aWarehouseDocumentViewing } from '../aWarehouseDocumentViewing';

/** Просмотр документа поступления на склад */
export class viewReceiptWarehouse extends aWarehouseDocumentViewing {
    static displayName = viewReceiptWarehouse.name;
    userFriendlyNameDocument = 'Поступление на склад';
}