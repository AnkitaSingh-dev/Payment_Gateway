using AutoMapper;

namespace Corno.Raychem.CustomerPortal
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                //Receipt
                //cfg.CreateMap<City, CityViewModel>();
                //cfg.CreateMap<CityViewModel, City>();

                //cfg.CreateMap<Country, CountryViewModel>();
                //cfg.CreateMap<CountryViewModel, Country>();

                //cfg.CreateMap<State, StateViewModel>();
                //cfg.CreateMap<StateViewModel, State>();

                //cfg.CreateMap<Company, CompanyViewModel>();
                //cfg.CreateMap<CompanyViewModel, Company>();

                //cfg.CreateMap<Zone, ZoneViewModel>();
                //cfg.CreateMap<ZoneViewModel, Zone>();
            });
        }
    }
}