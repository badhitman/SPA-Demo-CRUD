////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesReportsController : ControllerBase
    {
        public static Dictionary<int, string> AllowedReports = new Dictionary<int, string>()
        {
            {1, "Номенклатура->Склад" },
            {2, "Склад->Номенклатура" }
        };
        private readonly AppDataBaseContext _context;

        public WarehousesReportsController(AppDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/WarehousesReports
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetInventoryGoodsBalancesWarehouses()
        {
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос перечня доступных отчётов",
                Status = StylesMessageEnum.success.ToString(),
                Tag = AllowedReports.Select(x => new { id = x.Key, name = x.Value })
            });
        }

        // GET: api/WarehousesReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetInventoryBalancesWarehousesAnalyticalModel(int id)
        {
            ServerActionResult serverActionResult;
            var iBalance = await _context.InventoryGoodsBalancesWarehouses
                        .Include(x => x.Good)
                        .Include(x => x.Warehouse)
                        .Join(_context.Units, x => x.UnitId, x => x.Id, (balance, Unit) => new
                        {
                            balance.Good,
                            Unit,
                            balance.Quantity,
                            balance.Warehouse
                        })
                        .ToListAsync();

            switch (id)
            {
                case 1:// Номенклатура->Склад
                    //var t1 = iBalance
                    //    .GroupBy(iBalanceRow => iBalanceRow.Good.Name + " /" + iBalanceRow.Unit.Name,
                    //    (key, WarehousesValues) => new
                    //    {
                    //        key = new
                    //        {
                    //            key,
                    //            Sum = WarehousesValues.Sum(iBalanceSumRow => iBalanceSumRow.Quantity)
                    //        },
                    //        GroupValues = WarehousesValues.ToList().GroupBy(iBalanceSubRow => iBalanceSubRow.Warehouse.Name,
                    //        (subKey, subGroupValues) => new
                    //        {
                    //            Name = subKey,
                    //            Quantity = subGroupValues.ToList().Sum(x => x.Quantity)
                    //        }).ToList()
                    //    }).ToList();

                    serverActionResult = new ServerActionResult()
                    {
                        Success = true,
                        Info = AllowedReports[1],// Номенклатура->Склад
                        Status = StylesMessageEnum.success.ToString(),
                        Tag = iBalance
                        .GroupBy(iBalanceRow => iBalanceRow.Good.Name + " /" + iBalanceRow.Unit.Name,
                        (key, WarehousesValues) => new
                        {
                            key = new
                            {
                                Name = key,
                                Sum = WarehousesValues.Sum(iBalanceSumRow => iBalanceSumRow.Quantity)
                            },
                            GroupValues = WarehousesValues.GroupBy(iBalanceSubRow => iBalanceSubRow.Warehouse.Name,
                            (subKey, subGroupValues) => new
                            {
                                Name = subKey,
                                Quantity = subGroupValues.Sum(x => x.Quantity)
                            })
                        })
                    };
                    break;
                case 2:// "Склад->Номенклатура"
                    serverActionResult = new ServerActionResult()
                    {
                        Success = true,
                        Info = AllowedReports[2],// "Склад->Номенклатура"
                        Status = StylesMessageEnum.success.ToString(),
                        Tag = iBalance
                        .GroupBy(iBalanceRow => iBalanceRow.Warehouse.Name,
                        (key, GroupValues) => new
                        {
                            key = new
                            {
                                Name = key,
                                Sum = GroupValues.Sum(iBalanceSumRow => iBalanceSumRow.Quantity)
                            },
                            GroupValues = GroupValues.GroupBy(iBalanceSubRow => iBalanceSubRow.Good.Name + " /" + iBalanceSubRow.Unit.Name,
                            (subKey, subGroupValues) => new { subKey, subGroupValues }).Select(iBalanceSubRow => new
                            {
                                Name = iBalanceSubRow.subKey,
                                Quantity = iBalanceSubRow.subGroupValues.Sum(item => item.Quantity)
                            })
                        })
                    };
                    break;
                default:
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Запрашиваемого отчёта нет",
                        Status = StylesMessageEnum.danger.ToString()
                    });
            }
            return new ObjectResult(serverActionResult);
        }
    }
}
