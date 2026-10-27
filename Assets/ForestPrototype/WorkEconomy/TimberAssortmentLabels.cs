namespace CCF.Forestry.WorkEconomy
{
    // Player-facing names for timber products. The four Sitka products keep their enum names so existing
    // text is unchanged; the Oak/Beech products read as ordinary words rather than CamelCase identifiers.
    public static class TimberAssortmentLabels
    {
        public static string Label(TimberAssortment assortment)
        {
            switch (assortment)
            {
                case TimberAssortment.VeneerPlanking: return "First-grade veneer / planking";
                case TimberAssortment.Beam: return "Beam";
                case TimberAssortment.Fencing: return "Fencing material";
                case TimberAssortment.SecondPlanking: return "Second-quality planking";
                case TimberAssortment.Firewood: return "Firewood";
                default: return assortment.ToString();
            }
        }
    }
}
