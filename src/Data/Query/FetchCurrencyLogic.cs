using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoeCraftLib.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PoeCraftLib.Currency.Currency;

namespace PoeCraftLib.Data.Query
{
    public class FetchCurrencyLogic : IFetchFetchCurrencyLogic
    {
        private List<String> files = new List<string>()
        {
            "default_currency_logic.json",
            "conqueror_currency_logic.json",
            "quality_currency_logic.json"
        };
        public List<CurrencyLogicJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            return files.Select(x => FetchHelper.GetEmbeddedResource("Assets\\currency\\" + x, assem))
                .SelectMany(x => JsonConvert.DeserializeObject<List<CurrencyLogicJson>>(x, new CurrencyLogicConverter()))
                .ToList();
        }
    }

    public interface IFetchFetchCurrencyLogic : IQueryObject<List<CurrencyLogicJson>>
    {
    }

    public class CurrencyLogicConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanConvert is false. The type will skip the converter.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new InvalidOperationException("Expected array");
            }

            JToken token = JToken.Load(reader);
            return ReadToken(token);
        }

        private static List<CurrencyLogicJson> ReadToken(JToken token)
        {

            List<CurrencyLogicJson> currencyLogicList = new List<CurrencyLogicJson>();
            foreach (var item in token.Children().ToList())
            {
                String name = null;
                List<KeyValuePair<string, object>> steps = null;
                List<KeyValuePair<string, string>> requirements = null;
                List<KeyValuePair<string, string>> modifiers = null;

                foreach (var property in item.Children<JProperty>().ToList())
                {
                    switch (property.Name)
                    {
                        case "Name":
                            name = property.Value.ToObject<string>();
                            break;
                        case "Steps":
                            steps = GetSteps(property.Value.Children<JObject>().ToList());
                            break;
                        case "Modifiers":
                            modifiers = ParsePropertyList(property.Value.Children<JObject>().ToList());
                            break;
                        case "Requirements":
                            requirements = ParsePropertyList(property.Value.Children<JObject>().ToList());
                            break;
                    }
                }

                CurrencyLogicJson newObject = new CurrencyLogicJson
                {
                    Name = name,
                    Requirements = requirements,
                    Steps = steps,
                    Modifiers = modifiers
                };


                currencyLogicList.Add(newObject);
            }

            return currencyLogicList;
        }

        private static List<KeyValuePair<string, object>> GetSteps(List<JObject> stepsJson)
        {
            var stepsList = new List<KeyValuePair<string, object>>();
            foreach (JObject content in stepsJson)
            {
                foreach (JProperty prop in content.Properties())
                {
                    switch (prop.Value.Type)
                    {
                        case JTokenType.Array:
                            {
                                if (prop.Name == "RandomSteps")
                                {
                                    var steps = GetRandomSteps(prop.Children().Values().ToList());
                                    stepsList.Add(new KeyValuePair<string, object>(prop.Name, steps));
                                }

                                break;
                            }
                        case JTokenType.String:
                            stepsList.Add(new KeyValuePair<string, object>(prop.Name, prop.Value.ToObject<String>()));
                            break;
                    }
                }
            }

            return stepsList;
        }

        private static List<KeyValuePair<int, List<KeyValuePair<string, object>>>> GetRandomSteps(List<JToken> tokens)
        {
            var steps = new List<KeyValuePair<int, List<KeyValuePair<string, object>>>>();

            foreach (var token in tokens)
            {
                foreach (var prop in token.Values<JProperty>())
                {
                    var step = new KeyValuePair<int, List<KeyValuePair<string, object>>>(
                        int.Parse(prop.Name),
                        GetSteps(prop.Value.Children<JObject>().ToList()));

                    steps.Add(step);
                }
            }
            return steps;
        }

        private static List<KeyValuePair<string, string>> ParsePropertyList(List<JObject> properties)
        {
            var propertyList = new List<KeyValuePair<string, string>>();
            foreach (JObject content in properties)
            {
                foreach (JProperty prop in content.Properties())
                {
                    propertyList.Add(new KeyValuePair<string, string>(prop.Name, prop.Value.ToObject<String>()));
                }
            }

            return propertyList;
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
