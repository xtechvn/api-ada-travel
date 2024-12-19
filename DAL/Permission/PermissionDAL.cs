using DAL.Generic;
using DAL.StoreProcedure;
using ENTITIES.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using Utilities;

namespace DAL.Permission
{
    public class PermissionDAL : GenericService<Role>
    {
        private static DbWorker _DbWorker;
        public PermissionDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        // lấy ra nhóm useri của 1 quyền
        public DataTable getListUserByRoleId(int role_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@RoleId", role_id);               

                return _DbWorker.GetDataTable("Sp_GetListUserByRoleId", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PermissionDAL - GetPlayGroundDetail: " + ex);
            }
            return null;
        }
        // Lay ra userid của trưởng phòng 1 sale
        // input là nhân vien bán
        public DataTable getManagerByUserId (int user_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@UserId", user_id);

                return _DbWorker.GetDataTable("SP_GetManagerByUserId", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PermissionDAL - getManagerByUserId: " + ex);
            }
            return null;
        }
        // Lay ra userid của trưởng nhóm 1 sale
        // input là nhân vien bán
        public DataTable GetLeaderByUserId(int user_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@UserId", user_id);

                return _DbWorker.GetDataTable("Sp_GetLeaderByUserId", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PermissionDAL - getManagerByUserId: " + ex);
            }
            return null;
        }
        //Lấy ra nhân viên chính của 1 đơn hàng (số đơn hàng)
        public DataTable getSalerIdByOrderNo(string order_no)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderNo", order_no);

                return _DbWorker.GetDataTable("Sp_GetSalerIdByOrderNo", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PermissionDAL - getSalerIdByOrderNo: " + ex);
            }
            return null;
        }
        public DataTable getListOperatorByOrderNo(string order_no)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderNo", order_no);

                return _DbWorker.GetDataTable("Sp_getListOperatorByOrderNo", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PermissionDAL - getListOperatorByOrderNo: " + ex);
            }
            return null;
        }
        //Lấy ra nhân viên chính của 1 hop dong
        public DataTable getSalerIdByContractNo(string contract_no)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ContractNo", contract_no);

                return _DbWorker.GetDataTable("SP_GetUserIdCreateByContractNo", objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PermissionDAL - contract_no: " + ex);
            }
            return null;
        }

    }
}
