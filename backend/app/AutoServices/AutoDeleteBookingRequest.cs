namespace AutoServices;

public class AutoDeleteBookingRequest(ScriptService scriptService) : AutoService
{
    private const int DaysToExtenBooking = 7;
    private const int DaysToAcceptAExtention = 2;

    protected override async Task StartAuto(Context service, Config _config)
    {
        VmBookingService vmBookingService = new(service);
        VmBookingExtentionService extentionRequestService = new(service);
        foreach (VmBooking request in await vmBookingService.GetAllAsync())
        {
            VmBookingExtention? extention = await extentionRequestService.GetByBookingId(request.Id);

            //if (extention == null)
            //{
            //    await DeleteExpiredBooking(request, request.ExpiredAt
            //        .AddDays(DaysToExtenBooking), requestService);
            //    continue;
            //}
            //await DeleteExpiredBooking(request, request.ExpiredAt
            //    .AddDays(extention.IsAccepted ? 0 : DaysToAcceptAExtention), requestService);


            await DeleteExpiredBooking(request, request.ExpiredAt.AddDays(
                extention == null ? DaysToExtenBooking :
                extention.IsAccepted ? 0 : DaysToAcceptAExtention
            ), vmBookingService, scriptService);
        }
    }

    private static async Task DeleteExpiredBooking(VmBooking request, DateTime date, VmBookingService requestService, ScriptService _scriptExecutor)
    {
        if (date <= DateTime.UtcNow)
        {
            // AuthorizationValidator.ChechForDelveloment([
            //     async () => await _scriptExecutor.DeleteVMAsync(request.Uuid)
            // ]);
            await requestService.DeleteAsync(request);
        }
    }
}
