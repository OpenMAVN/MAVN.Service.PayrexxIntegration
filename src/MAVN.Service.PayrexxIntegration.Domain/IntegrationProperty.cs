namespace MAVN.Service.PayrexxIntegration.Domain
{
    public class IntegrationProperty
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string JsonKey { get; set; }
        public bool IsOptional { get; set; }
        public bool IsSecret { get; set; }
    }
}
