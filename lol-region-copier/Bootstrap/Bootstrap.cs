using lol_region_copier.Core;

namespace lol_region_copier.Bootstrap
{
	public partial class Bootstrap
	{
		private static void Main(string[] arguments)
		{
			LoLRegionCopier regionCopier = new LoLRegionCopier();
			regionCopier.Start();
		}
	}
}