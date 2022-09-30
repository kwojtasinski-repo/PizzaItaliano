namespace PizzaItaliano.Services.Products.Core.Exceptions
{
    public class ProductNameShouldHaveAtLeastThreeCharactersException : DomainException
    {
        public override string Code => "product_name_should_have_at_least_three_characters";
        public string Name { get; }

        public ProductNameShouldHaveAtLeastThreeCharactersException(string name) : base("Product name should have at least three characters")
        {
            Name = name;
        }
    }
}
