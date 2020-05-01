////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace SPADemoCRUD.Models
{
    public abstract class BodyGoodMovementDocumentModel : ObjectFileRegisterModel
    {
        public int AuthorId { get; set; }
        /// <summary>
        /// Автор документа
        /// </summary>
        public UserObjectModel Author { get; set; }

        public string Discriminator { get; set; }

        /// <summary>
        /// Строки документа
        /// </summary>
        public List<RowGoodMovementRegisterModel> RowsDocument { get; set; }

        /// <summary>
        /// Формирование анономного/динамического объекта: документ-владелец записи регистра.
        /// </summary>
        /// <param name="document">объект-документ, строки которого представляют записи регистров</param>
        /// <param name="context">контекст базы данных</param>
        /// <returns>возвращается объект-документ-владелец записи регистра в виде объекта анонимного типа</returns>
        public static object getDocument(BodyGoodMovementDocumentModel document, AppDataBaseContext context)
        {
            switch (document.Discriminator)
            {
                case nameof(ReceiptToWarehouseDocumentModel):
                    ReceiptToWarehouseDocumentModel movDocument = context.ReceiptesGoodsToWarehousesDocuments
                        .Where(x => x.Id == document.Id)
                        .Include(x => x.Warehouse)
                        .FirstOrDefault();

                    return new
                    {
                        document.DateCreate,
                        document.Id,
                        document.Name,
                        document.Information,
                        document.Discriminator,
                        Warehouse = new
                        {
                            movDocument.Warehouse.Id,
                            movDocument.Warehouse.Name,
                            movDocument.Warehouse.Information
                        },
                        Author = new
                        {
                            document.Author.Id,
                            document.Author.Name,
                            document.Author.Information
                        }
                    };
                case nameof(InternalDisplacementWarehouseDocumentModel):
                    InternalDisplacementWarehouseDocumentModel InternalDisplacementWarehouseDocument = context.InternalDisplacementWarehouseDocuments.Include(x => x.Author).Include(x => x.Warehouse).FirstOrDefault(x => x.Id == document.Id);
                    WarehouseObjectModel WarehouseDebiting = context.Warehouses.Find(InternalDisplacementWarehouseDocument.WarehouseDebitingId);
                    //case nameof(InternalDisplacementWarehouseDocumentModel):

                    //    WarehouseObjectModel WarehouseDebiting = context.Warehouses.Find(((InternalDisplacementWarehouseDocumentModel)document).WarehouseDebitingId);

                    //    return new
                    //    {
                    //        document.Id,
                    //        document.Name,
                    //        document.Information,
                    //        document.Discriminator,
                    //        CountRows = context.GoodMovementDocumentRows.Count(x => x.BodyDocumentId == document.Id),
                    //        Author = new
                    //        {
                    //            document.Author.Id,
                    //            document.Author.Name
                    //        },
                    //        WarehouseDebiting = new
                    //        {
                    //            WarehouseDebiting.Id,
                    //            WarehouseDebiting.Name
                    //        },
                    //        WarehouseReceipt = new
                    //        {
                    //            document.Warehouse.Id,
                    //            document.Warehouse.Name
                    //        }
                    //    };
                    return new
                    {
                        document.DateCreate,
                        author = new { id = document.AuthorId, document.Author.Name },
                        document.Id,
                        document.Name,
                        document.Information,
                        document.Discriminator,
                        about = "Поступление на склад",
                        WarehouseDebiting = new { WarehouseDebiting.Id, WarehouseDebiting.Name },
                        WarehouseReceipt = new { InternalDisplacementWarehouseDocument.Warehouse.Id, InternalDisplacementWarehouseDocument.Warehouse.Name, InternalDisplacementWarehouseDocument.Warehouse.Information }
                    };
                case nameof(MovementTurnoverDeliveryDocumentModel):
                    MovementTurnoverDeliveryDocumentModel MovementTurnoverDeliveryDocument = context.MovementTurnoverDeliveryDocuments.Include(x => x.DeliveryService).Include(x => x.DeliveryMethod).Include(x => x.Buyer).FirstOrDefault(x => x.Id == document.Id);
                    //case nameof(MovementTurnoverDeliveryDocumentModel):

                    //    MovementTurnoverDeliveryDocumentModel MovementTurnoverDeliveryDocument = context.MovementTurnoverDeliveryDocuments
                    //        .Include(x => x.Buyer)
                    //        .Include(x => x.DeliveryMethod)
                    //        .Include(x => x.DeliveryService)
                    //        .FirstOrDefault(x => x.Id == document.Id);

                    //    return new
                    //    {
                    //        document.Id,
                    //        document.Name,
                    //        document.Information,
                    //        document.Discriminator,
                    //        CountRows = context.GoodMovementDocumentRows.Count(row => row.BodyDocumentId == document.Id),
                    //        Author = new
                    //        {
                    //            document.Author.Id,
                    //            document.Author.Name,
                    //            document.Author.Information
                    //        },
                    //        WarehouseReceipt = new
                    //        {
                    //            document.Warehouse.Id,
                    //            document.Warehouse.Name,
                    //            document.Warehouse.Information
                    //        },
                    //        Buyer = new
                    //        {
                    //            MovementTurnoverDeliveryDocument.Buyer.Id,
                    //            MovementTurnoverDeliveryDocument.Buyer.Name,
                    //            MovementTurnoverDeliveryDocument.Buyer.Information
                    //        },
                    //        DeliveryMethod = new
                    //        {
                    //            MovementTurnoverDeliveryDocument.DeliveryMethod.Id,
                    //            MovementTurnoverDeliveryDocument.DeliveryMethod.Name
                    //        },
                    //        DeliveryService = new
                    //        {
                    //            MovementTurnoverDeliveryDocument.DeliveryService.Id,
                    //            MovementTurnoverDeliveryDocument.DeliveryService.Name
                    //        },
                    //        MovementTurnoverDeliveryDocument.DeliveryAddress1,
                    //        MovementTurnoverDeliveryDocument.DeliveryAddress2
                    //    };
                    return new
                    {
                        document.DateCreate,
                        author = new { id = document.AuthorId, document.Author.Name },
                        document.Id,
                        document.Name,
                        document.Information,
                        document.Discriminator,
                        about = "Отгрузка/Доставка",
                        buyer = new
                        {
                            MovementTurnoverDeliveryDocument.Buyer?.Id,
                            MovementTurnoverDeliveryDocument.Buyer?.Name
                        },
                        deliveryMethod = new
                        {
                            MovementTurnoverDeliveryDocument.DeliveryMethod?.Id,
                            MovementTurnoverDeliveryDocument.DeliveryMethod?.Name
                        },
                        deliveryService = new
                        {
                            MovementTurnoverDeliveryDocument.DeliveryService?.Id,
                            MovementTurnoverDeliveryDocument.DeliveryService?.Name
                        },
                        MovementTurnoverDeliveryDocument.DeliveryAddress1,
                        MovementTurnoverDeliveryDocument.DeliveryAddress2
                    };
                default:
                    //default:
                    //    return new
                    //    {
                    //        document.Id,
                    //        document.Name,
                    //        document.Information,
                    //        document.Discriminator,
                    //        CountRows = context.GoodMovementDocumentRows.Count(x => x.BodyDocumentId == document.Id),
                    //        Author = new
                    //        {
                    //            document.Author.Id,
                    //            document.Author.Name
                    //        },
                    //        WarehouseReceipt = new
                    //        {
                    //            document.Warehouse.Id,
                    //            document.Warehouse.Name
                    //        }
                    //    };
                    return new
                    {
                        document.Id,
                        document.Name,
                        document.Information,
                        document.Discriminator,
                        author = new
                        {
                            document.Author.Id,
                            document.Author.Name
                        }
                    };
            }
        }
    }
}
