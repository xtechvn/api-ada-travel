using DAL.Generic;
using System;
using ENTITIES.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace DAL
{
    public class VoucherDAL : GenericService<Voucher>
    {
        public VoucherDAL(string connection) : base(connection)
        {
        }

        public async Task<Voucher> FindByVoucherCode(string voucherCode)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Voucher.FirstOrDefaultAsync(s => s.Code.ToUpper() == voucherCode.ToUpper());
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByVoucherCode - VoucherDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<Voucher> FindByVoucheId(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Voucher.FirstOrDefaultAsync(s => s.Id == id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByVoucherCode - VoucherDAL: " + ex.ToString());
                return null;
            }
        }

        public async Task<Voucher> FindByVoucherCode(string voucherCode, bool is_public = false)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Voucher.FirstOrDefaultAsync(s => s.Code.ToUpper() == voucherCode.ToUpper() && s.IsPublic == is_public);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByVoucherCode - VoucherDAL: " + ex);
                return null;
            }
        }
        public async Task<string> FindByVoucherid(int voucherId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    
                     var Voucher= await _DbContext.Voucher.FirstOrDefaultAsync(s => s.Id == voucherId);
                    return Voucher.Code;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByVoucherCode - VoucherDAL: " + ex.ToString());
                return null;
            }
        }
    }
}
