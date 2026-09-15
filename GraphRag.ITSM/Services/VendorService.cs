using GraphRag.ITSM.Context;
using GraphRag.ITSM.Entities;

namespace GraphRag.ITSM.Services;

public class VendorService : EntityService<Vendor>
{
    public VendorService(ItsmDbContext dbContext) : base(dbContext) { }
}

