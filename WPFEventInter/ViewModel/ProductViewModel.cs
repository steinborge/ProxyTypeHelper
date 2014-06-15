using ProxyHelper;

namespace WPFEventInter.ViewModel
{
    public class ProductViewModel : ProxyTypeHelper<ProductViewModel>
    {
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
