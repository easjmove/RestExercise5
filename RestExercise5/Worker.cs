using RestExercise5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RestExercise5
{
    public class Worker
    {
        //The url from RestExercise5
        public static string BaseURL = "http://localhost:28475/api/items";

        public void Start()
        {
            Console.WriteLine("Calling Get with no parameters");

            //Calling the GetAllItems to receive all items from the server
            //Calls .Result to make the call synchronous (wait for the wait result before continuing the code 
            IEnumerable<Item> allItems = GetAllItems().Result;
            //iterates all the items in the list and uses the ToString method to the console
            foreach (Item myItem in allItems)
            {
                Console.WriteLine(myItem.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Calling Get with all parameters");

            //Executes the filter with parameters that should return the first item in the list
            IEnumerable<Item> allFilteredItems = GetAllItems("book", 100, 5).Result;
            //iterates all the items in the list and uses the ToString method to the console
            //Should only be only in this instance but it returns a list
            foreach (Item myItem in allFilteredItems)
            {
                Console.WriteLine(myItem.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Calling Get with quality parameters");

            //Executes the filter with parameters that should return 2 items
            IEnumerable<Item> allQualityItems = GetAllItems(20, 500).Result;
            //iterates all the items in the list and uses the ToString method to the console
            foreach (Item myItem in allQualityItems)
            {
                Console.WriteLine(myItem.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Calling GetByID");

            //Executes the filter with parameters that should return 2 items
            Item specificItem = GetItemByID(2).Result;
            Console.WriteLine(specificItem.ToString());

            Console.WriteLine();
            Console.WriteLine("Calling PostItem");

            //a new item that should be added to the server
            Item newItem = new Item() { Name = "new TestItem", ItemQuality = 5, Quantity = 8 };
            //stores the returned item in a new variable that has the new ID from the server
            Item newItemReturned = PostItem(newItem).Result;
            Console.WriteLine(newItemReturned.ToString());

            Console.WriteLine();
            Console.WriteLine("Calling PutItem");

            //a new item that contains values the server should update for the id of the previous returned Item
            Item updateItem = new Item() { Name = "updated item", ItemQuality = 4, Quantity = 7 };
            //stores the returned item in a new variable that has the new ID from the server
            Item updateItemReturned = PutItem(newItemReturned.Id, updateItem).Result;
            Console.WriteLine(updateItemReturned.ToString());

            Console.WriteLine();
            Console.WriteLine("Calling DeleteItem");

            //Calls the deleteitem with the ID from the posted Item
            //stores the returned item in a new variable that has the new ID from the server
            Item deletedItemReturned = DeleteItem(newItemReturned.Id).Result;
            Console.WriteLine(deletedItemReturned.ToString());

            Console.WriteLine();
            Console.WriteLine("Calling DeleteItem with an already deleted id");

            try
            {
                Item notFoundDeletedItemReturned = DeleteItem(newItemReturned.Id).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception happened: " + ex.Message);
            } 
        }

        public async Task<IEnumerable<Item>> GetAllItems()
        {
            //initializes a HttpClient to be used
            //here it is initialized in a using statement, so the code cleans up the connections (calling Dispose method)
            using (HttpClient client = new HttpClient())
            {
                //calls the baseurl with no parameters
                //this makes the Rest service return all Items with no filter
                HttpResponseMessage response = await client.GetAsync(BaseURL);
                //checks for a 200 or a 204. 200 is what we want but if a 204 is returned, the list will be initialized but empty
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    //The ReadFromJson part deserializes the response automatically
                    //instead of explicitly doing it in the code
                    IEnumerable<Item> items = await response.Content.ReadFromJsonAsync<IEnumerable<Item>>();
                    return items;
                }
                else
                {
                    //If not 200 or 204 throws an exception with the status code and message from the server
                    throw new Exception($"Unknown error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            //No need to close connection or dispose, because of the using surrounding the HttpClient
        }

        //An improvement that could be made here, is to make the parameters nullable, and then only add them if they have values
        //In order to make this work, it should check if its the first parameter then add a ? else a &
        public async Task<IEnumerable<Item>> GetAllItems(string substring, int minimumQuality, int minimumQuantity)
        {
            //initializes a HttpClient to be used
            //here it is initialized in a using statement, so the code cleans up the connections (calling Dispose method)
            using (HttpClient client = new HttpClient())
            {
                //calls the baseurl and adds the parameters
                //this makes the Rest service return all Items after the filter is applied
                HttpResponseMessage response = await client.GetAsync(BaseURL + $"?substring={substring}&minimumQuality={minimumQuality}&minimumQuantity={minimumQuantity}");
                //checks for a 200 or a 204. 200 is what we want but if a 204 is returned, the list will be initialized but empty
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    //The ReadFromJson part deserializes the response automatically
                    //instead of explicitly doing it in the code
                    IEnumerable<Item> items = await response.Content.ReadFromJsonAsync<IEnumerable<Item>>();
                    return items;
                }
                else
                {
                    //If not 200 or 204 throws an exception with the status code and message from the server
                    throw new Exception($"Unknown error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            //No need to close connection or dispose, because of the using surrounding the HttpClient
        }

        public async Task<IEnumerable<Item>> GetAllItems(int minimumQuality, int maximumQuality)
        {
            //initializes a HttpClient to be used
            //here it is initialized in a using statement, so the code cleans up the connections (calling Dispose method)
            using (HttpClient client = new HttpClient())
            {
                //calls the baseurl and adds /quality to the Uri and then adds the parameters
                HttpResponseMessage response = await client.GetAsync(BaseURL + $"/quality?minQuality={minimumQuality}&maxQuality={maximumQuality}");
                //checks for a 200 or a 204. 200 is what we want but if a 204 is returned, the list will be initialized but empty
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    //The ReadFromJson part deserializes the response automatically
                    //instead of explicitly doing it in the code
                    IEnumerable<Item> items = await response.Content.ReadFromJsonAsync<IEnumerable<Item>>();
                    return items;
                }
                else
                {
                    //If not 200 or 204 throws an exception with the status code and message from the server
                    throw new Exception($"Unknown error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        public async Task<Item> GetItemByID(int id)
        {
            //initializes a HttpClient to be used
            //here it is initialized in a using statement, so the code cleans up the connections (calling Dispose method)
            using (HttpClient client = new HttpClient())
            {
                //calls the baseurl and adds /id to the Uri
                HttpResponseMessage response = await client.GetAsync(BaseURL + $"/{id}");
                //checks for a 200, meaning we got an Item
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //The ReadFromJson part deserializes the response automatically
                    //instead of explicitly doing it in the code
                    Item item = await response.Content.ReadFromJsonAsync<Item>();
                    return item;
                }
                //checks for a 404, meaning the server doens't have an Item with the ID
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"The id: {id} was not found");
                }
                else
                {
                    //If not 200 or 404 throws an exception with the status code and message from the server
                    throw new Exception($"Unknown error: {response.StatusCode} - {response.ReasonPhrase}");
                }

            }
        }

        public async Task<Item> PostItem(Item newItem)
        {
            //initializes a HttpClient to be used
            //here it is initialized in a using statement, so the code cleans up the connections (calling Dispose method)
            using (HttpClient client = new HttpClient())
            {
                //Serializes the Item, and wraps it in a JsonContent that can easily be used with the HttpClient
                JsonContent serializedItem = JsonContent.Create(newItem);
                //calls the baseurl and the data is added to the body
                HttpResponseMessage response = await client.PostAsync(BaseURL, serializedItem);
                //checks for a 201, meaning the Item got posted sucessfully
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    //The ReadFromJson part deserializes the response automatically
                    //instead of explicitly doing it in the code
                    Item item = await response.Content.ReadFromJsonAsync<Item>();
                    return item;
                }
                else
                {
                    //If not 201 or 404 throws an exception with the status code and message from the server
                    throw new Exception($"Unknown error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        public async Task<Item> PutItem(int id, Item newItem)
        {
            //initializes a HttpClient to be used
            //here it is initialized in a using statement, so the code cleans up the connections (calling Dispose method)
            using (HttpClient client = new HttpClient())
            {
                //Serializes the Item, and wraps it in a JsonContent that can easily be used with the HttpClient
                JsonContent serializedItem = JsonContent.Create(newItem);
                //calls the baseurl and adds the id to the URI and the data is added to the body
                HttpResponseMessage response = await client.PutAsync(BaseURL + $"/{id}", serializedItem);

                //checks for a 200, meaning the Item got updated
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //The ReadFromJson part deserializes the response automatically
                    //instead of explicitly doing it in the code
                    Item item = await response.Content.ReadFromJsonAsync<Item>();
                    return item;
                }
                //checks for a 404, meaning the server doens't have an Item with the ID
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"The id: {id} was not found");
                }
                else
                {
                    //If not 200 or 404 throws an exception with the status code and message from the server
                    throw new Exception($"Unknown error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        public async Task<Item> DeleteItem(int id)
        {
            //initializes a HttpClient to be used
            //here it is initialized in a using statement, so the code cleans up the connections (calling Dispose method)
            using (HttpClient client = new HttpClient())
            {
                //calls the baseurl and adds the id to the URI
                HttpResponseMessage response = await client.DeleteAsync(BaseURL + $"/{id}");

                //checks for a 200, meaning the Item got deleted
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //The ReadFromJson part deserializes the response automatically
                    //instead of explicitly doing it in the code
                    Item item = await response.Content.ReadFromJsonAsync<Item>();
                    return item;
                }
                //checks for a 404, meaning the server doens't have an Item with the ID
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"The id: {id} was not found");
                }
                else
                {
                    //If not 200 or 404 throws an exception with the status code and message from the server
                    throw new Exception($"Unknown error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
    }
}