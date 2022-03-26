﻿using System;
using System.Text;

namespace CocktailParty
{
    public class Ingredient
    {
        public Ingredient(string name, int alcohol, int quantity)
        {
            this.Name = name;
            this.Alcohol = alcohol;
            this.Quantity = quantity;
        }

        public string Name { get; set; }

        public int Alcohol { get; set; }

        public int Quantity { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb
              .AppendLine($"Ingredient: {this.Name}")
              .AppendLine($"Quantity: {this.Quantity}")
              .AppendLine($"Alcohol: {this.Alcohol}");

            return sb.ToString().TrimEnd();
        }
    }
}
