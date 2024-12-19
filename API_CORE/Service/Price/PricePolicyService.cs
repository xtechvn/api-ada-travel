using API_CORE.Model.PricePolicy;
using ENTITIES.APIModels;
using ENTITIES.Models;
using ENTITIES.ViewModels.FlyTicket;
using ENTITIES.ViewModels.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Contants;
using static MongoDB.Driver.WriteConcern;

namespace API_CORE.Service.Price
{
    public static class PricePolicyService
    {
        public static double CalucateMinProfit(List<HotelPricePolicyViewModel> policy_list, double amount, DateTime arrival_date, DateTime departure_date)
        {
            double profit = 0;
            try
            {
                if (policy_list != null && policy_list.Count > 0)
                {
                    int nights = Convert.ToInt32((departure_date - arrival_date).TotalDays < 1 ? 1 : (departure_date - arrival_date).TotalDays);
                    double price = amount / (double)nights;
                    List<double> actual_profit = new List<double>();
                    for (int d = 0; d < nights; d++)
                    {
                        var stay_date = arrival_date.AddDays(d);
                        var day_of_week = stay_date.DayOfWeek;
                        var selected_policy = policy_list/*.Where(x => x.FromDate <= stay_date && x.ToDate >= stay_date )*/;
                        List<double> profit_day = new List<double>();
                        foreach (var policy in selected_policy)
                        {
                            double item_profit = 0;
                            switch (policy.UnitId)
                            {
                                case (int)PriceUnitType.VND:
                                    {
                                        item_profit = policy.Profit;
                                    }
                                    break;
                                case (int)PriceUnitType.PERCENT:
                                    {
                                        item_profit = Math.Round((price * policy.Profit / (double)100), 0);
                                    }
                                    break;
                            }
                            profit_day.Add(item_profit);
                        }
                        if (profit_day.Count > 0)
                        {
                            actual_profit.Add(profit_day.OrderBy(x => x).First());
                        }
                    }
                    if (actual_profit.Count > 0)
                    {
                        profit = actual_profit.Sum(x => x);
                    }
                }

            }
            catch
            {

            }
            return profit;
        }

        public static FlightServicePriceModel GetFlyTicketProfit(List<FlyPricePolicyViewModel> policy_list, double amount)
        {
            try
            {
                List<PricePolicyServiceModel> data = new List<PricePolicyServiceModel>();
                if (policy_list != null && policy_list.Count > 0)
                {
                    foreach (var policy in policy_list)
                    {
                        double item_profit = 0;
                        switch (policy.UnitId)
                        {
                            case (int)PriceUnitType.VND:
                                {
                                    item_profit = policy.Profit;
                                }
                                break;
                            case (int)PriceUnitType.PERCENT:
                                {
                                    item_profit = Math.Round((amount * policy.Profit / (double)100), 0);
                                }
                                break;
                        }
                        data.Add(new PricePolicyServiceModel()
                        {
                            PriceID = policy.PriceDetailId,
                            profit = item_profit
                        });
                    }
                }
                if (data.Count > 0)
                {
                    var min_data = data.OrderBy(x => x.profit).First();
                    FlightServicePriceModel model = new FlightServicePriceModel()
                    {
                        price_id = min_data.PriceID,
                        profit = min_data.profit,
                        amount = min_data.profit + amount,
                        price = min_data.profit + amount
                    };
                    return model;
                }

            }
            catch (Exception)
            {

            }
            return new FlightServicePriceModel()
            {
                price_id = 0,
                amount = amount,
                price = amount,
                profit = 0
            };
        }
        public static RoomDetail GetRoomDetail(string room_id, DateTime arrival_date, DateTime departure_date, int nights, List<HotelFERoomPackageDataModel> room_packages_daily, List<HotelFERoomPackageDataModel> room_packages_special, List<HotelPricePolicyViewModel> profit_list, Hotel hotel, RoomDetail exist_detail = null, int client_type = 1)
        {
            RoomDetail result = new RoomDetail();
            try
            {
                if (exist_detail != null) result = exist_detail;
                List<RoomDetailRate> all_rates = new List<RoomDetailRate>();
                if ((room_packages_special == null && room_packages_daily == null) || room_packages_special.Count <= 0 && room_packages_daily.Count <= 0)
                    return result;
                //-- Get List Package Code name:
                List<PricePolicyFilterModel> package_codes = new List<PricePolicyFilterModel>();
                if (room_packages_special.Count > 0)
                {
                    package_codes.AddRange(room_packages_special.Select(x => new PricePolicyFilterModel() { PackageCode=x.PackageCode, ProgramId=x.ProgramId }));
                }
                if (room_packages_daily.Count > 0)
                {
                    package_codes.AddRange(room_packages_daily.Select(x => new PricePolicyFilterModel() { PackageCode = x.PackageCode, ProgramId = x.ProgramId }));
                }
                package_codes = package_codes.Distinct().ToList();
                for (int d = 0; d < nights; d++)
                {
                    var stay_date = arrival_date.AddDays(d);
                    var day_of_week_orginal = (int)stay_date.DayOfWeek;
                    //-- Set from sunday: 0,2,3,4,5,6,7
                    var day_of_week = day_of_week_orginal <= 0 ? 0 : (day_of_week_orginal + 1);
                    foreach (var code in package_codes)
                    {
                        //-- Rate set riêng
                        var rate_special = room_packages_special.Where(x => x.ApplyDate == stay_date && x.PackageCode == code.PackageCode && x.ProgramId==code.ProgramId).OrderBy(x => x.Amount).FirstOrDefault();
                        if (rate_special != null)
                        {
                            var profit = profit_list.Where(x => x.RoomId.ToString() == room_id && x.PackageCode.ToString().Trim().ToLower() == rate_special.PackageCode.Trim().ToLower() && x.ProgramId == rate_special.ProgramId).ToList();
                            //-- Không có lợi nhuận thì không hiển thị
                            double min_profit = 0;
                            //-- Nếu là nhân viên thì tính lợi nhuận =0 để nhân viên check giá
                            if (client_type == (int)ClientType.STAFF)
                            {

                            }
                            else
                            {
                                if (profit == null || profit.Count <= 0)
                                {
                                    all_rates = all_rates.Where(x => x.code != code.PackageCode && x.program_id == code.ProgramId).ToList();
                                    continue;
                                }
                                min_profit = CalucateProfitPerDay(profit, rate_special.Amount);
                            }

                            all_rates.Add(new RoomDetailRate()
                            {
                                allotment_id = "",
                                apply_date = stay_date,
                                cancel_policy = new List<string>(),
                                code = rate_special.PackageCode,
                                description = "",
                                guarantee_policy = rate_special.Description,
                                id = rate_special.Id.ToString(),
                                name = rate_special.PackageName,
                                total_price = rate_special.Amount + min_profit,
                                total_profit = min_profit,
                                amount = rate_special.Amount,
                                program_name = rate_special.ProgramName
                            });
                        }
                        else
                        {
                            //-- Rate thường ngày
                            var rate_daily = room_packages_daily.Where(x => x.PackageCode == code.PackageCode && x.ProgramId == code.ProgramId && x.WeekDay == day_of_week && stay_date >= x.FromDate && stay_date <= x.ToDate).OrderBy(x => x.Amount).ToList();
                            //-- Nếu không có cả 2 rate, thì loại bỏ gói vì ko đủ ngày
                            if (rate_daily == null || rate_daily.Count <= 0) {
                                all_rates = all_rates.Where(x => x.code != code.PackageCode && x.program_id == code.ProgramId).ToList();
                                continue;
                            }

                            List<RoomDetailRate> rd_daily_calculated = new List<RoomDetailRate>();
                            foreach (var rd in rate_daily)
                            {
                                if (rate_daily == null)
                                {
                                    continue;
                                }
                                double min_profit = 0;
                                //-- Nếu là nhân viên thì tính lợi nhuận =0 để nhân viên check giá
                                if (client_type == (int)ClientType.STAFF)
                                {

                                }
                                else
                                {
                                    //-- Không có lợi nhuận thì không hiển thị
                                    var profit = profit_list.Where(x => x.RoomId == rd.RoomTypeId && x.PackageCode.ToString().Trim() == rd.PackageCode && x.ProgramId == rd.ProgramId).ToList();
                                    if (profit == null || profit.Count <= 0)
                                    {
                                        continue;
                                    }
                                    min_profit = CalucateProfitPerDay(profit, rd.Amount);
                                }

                                rd_daily_calculated.Add(new RoomDetailRate()
                                {
                                    allotment_id = "",
                                    apply_date = stay_date,
                                    cancel_policy = new List<string>(),
                                    code = rd.PackageCode,
                                    description = "",
                                    guarantee_policy = rd.Description,
                                    id = rd.Id.ToString(),
                                    name = rd.PackageName,
                                    total_price = rd.Amount + min_profit,
                                    total_profit = min_profit,
                                    amount = rd.Amount,
                                    program_name = rd.ProgramName,
                                   
                                });
                            }
                            if (rd_daily_calculated.Count <= 0)
                            {
                                all_rates = all_rates.Where(x => x.code != code.PackageCode && x.program_id == code.ProgramId).ToList();
                                continue;
                            }
                            else
                            {
                                all_rates.Add(rd_daily_calculated.OrderBy(x => x.amount).First());
                            }
                        }
                    }
                }

                result.rates = all_rates.Where(x => x.total_price > 0).GroupBy(s => new { s.code, s.program_name }).Select(s => new RoomDetailRate
                {
                    id = $"{room_id}-{s.Select(i => i.id).FirstOrDefault()}".ToLower(),
                    code = s.Key.code,
                    name = s.Select(i => i.name).FirstOrDefault(),
                    description = s.Select(i => i.description).FirstOrDefault(),
                    total_price = s.Average(k => k.total_price) * nights,
                    total_profit = s.Average(k => k.total_profit) * nights,
                    amount = s.Average(k => k.amount) * nights,
                    program_name = s.Select(i => i.program_name).FirstOrDefault(),
                    guarantee_policy= all_rates[0].guarantee_policy

                }).ToList();
                // Tính min_price
                if (result.rates != null && result.rates.Count > 0)
                {

                    result.min_price = result.rates.Count > 0 ? result.rates.OrderBy(x => x.total_price).First().total_price : 0;
                }
                else
                {
                    result.rates = new List<RoomDetailRate>();
                    result.min_price = 0;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public static double CalucateProfitPerDay(List<HotelPricePolicyViewModel> policy_list, double amount)
        {
            double profit = 0;
            try
            {
                if (policy_list != null && policy_list.Count > 0)
                {
                    List<double> actual_profit = new List<double>();
                    foreach (var policy in policy_list)
                    {
                        double item_profit = 0;
                        switch (policy.UnitId)
                        {
                            case (int)PriceUnitType.VND:
                                {
                                    item_profit = policy.Profit;
                                }
                                break;
                            case (int)PriceUnitType.PERCENT:
                                {
                                    item_profit = Math.Round((amount * policy.Profit / (double)100), 0);
                                }
                                break;
                        }
                        actual_profit.Add(item_profit);
                    }
                    if (actual_profit.Count > 0)
                    {
                        profit = actual_profit.OrderBy(x => x).First();
                    }
                }

            }
            catch
            {

            }
            return profit;
        }
    }
  
}


