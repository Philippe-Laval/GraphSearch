using ITSM.Web.Client.Models;

namespace ITSM.Web.Client.Services.DataProviders;

public class ChartAnnotationsProvider(Action onChanged)
{
    public List<Annotation> ChartAnnotations { get; } = [];

    public void Notify() {
        onChanged();
    }
}
