using Sufinn.Visitor.Core.Common;
using Sufinn.Visitor.Core.Model;
using Sufinn.Visitor.Repository.Interface;
using Sufinn.Visitor.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class AutoCheckoutService
{
    private readonly IBaseService _service;
    private readonly IPostgreRepository _repo;

    public AutoCheckoutService(IBaseService service, IPostgreRepository repo)
    {
        _service = service;
        _repo = repo;
    }
    public async Task<Result<string>> AutoCheckout()
    {
        try
        {
            var uncheckedVisitors = _service.GetRespository<TxnVisitorDetail>()
                                            .Filter(x => x.CheckOut == null);
            if (uncheckedVisitors.ToList().Count == 0)
            {
                return Common<string>.getResponse(false, "No Unchecked User Found", null);
            }
            foreach (var visitor in uncheckedVisitors)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "mobile_num", visitor.Number },
                    { "check_out_date", DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss") },
                    { "is_force_c_out", 0 },
                    { "id", visitor.TxnId },
                };
                await _repo.Save("pr_checkout_visitor", parameters);
            }

            return Common<string>.getResponse(true, "Auto checkout completed", null);
        }
        catch (Exception ex)
        {
            return Common<string>.getResponse(false, ex.Message, null);
        }
    }

}
