using MODELS.BASE;
using MODELS.USER.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.FRIENDREQUEST.Dtos
{
    public class MODELFriendRequest : MODELBase
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        /// <summary>
        /// 0: Chưa xác nhận, 1: Đồng ý, 2: Từ chối 
        /// </summary>
        public int Status { get; set; }
        
        public MODELUser User { get; set; }


        #region Hàm hỗ trợ
        public int Duration => (int)((DateTime.Now - NgayTao).Value.TotalHours);
        #endregion
    }
}
