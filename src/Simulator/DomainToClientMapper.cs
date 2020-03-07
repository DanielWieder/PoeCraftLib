using AutoMapper;

namespace PoeCraftLib.Simulator
{
    public class DomainToClientMapper
    {
        public IMapper GenerateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Entities.Items.Equipment, Model.Items.Equipment>();
                cfg.CreateMap<Entities.Items.EquipmentRarity, Model.Items.EquipmentRarity>();
                cfg.CreateMap<Entities.Items.Influence, Model.Items.Influence>();
                cfg.CreateMap<Entities.Items.ItemBase, Model.Items.ItemBase>();
                cfg.CreateMap<Entities.Items.Stat, Model.Items.Stat>();
                cfg.CreateMap<Entities.Affix, Model.Items.Affix>();
            });

            return configuration.CreateMapper();
        }
    }
}
