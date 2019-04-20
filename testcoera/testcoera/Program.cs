using System;
using System.Collections.Generic;
using System.Linq;

namespace testcoera
{
    class Player
    {
        static void Main(string[] args)
        {
            Player player = new Player();
            DateTime currentDay = new DateTime(2019, 3, 19);

            player.addCard(new Silver(new DateTime(2020, 5, 23)));
            player.addMoney("Silver", 4000);
            player.addCard(new Gold(new DateTime(2018, 8, 15)));
            player.addMoney("Gold", 2000);
            player.addCard(new Platinum(new DateTime(2019, 3, 20)));
            player.addMoney("Platinum", 3000);
            player.addCard(new Iridium(new DateTime(2020, 6, 23)));
            player.addMoney("Iridium", 5000);
            player.addCard(new Bronze(new DateTime(2019, 7, 15)));
            player.addMoney("Bronze", 2500);
            player.addCard(new Premium(new DateTime(2019, 8, 20)));
            player.addMoney("Premium", 2000);

            List<Card> validcards = player.GetValidCards(currentDay);
            Dictionary<Card, Tuple<float, float>> TVA1 = player.GetCardsCost(10000, currentDay);
            foreach (var i in TVA1)
            {
                Console.Write(i.Key.Name + ": ");
                Console.WriteLine("\nTVA: " + i.Value.Item1 + "\tFEE: " + i.Value.Item2);
            }
        }

        public List<Card> cards;
        public Player()
        {
            this.cards = new List<Card>();
        }
        public Player(List<Card> cards)
        {
            this.cards = new List<Card>();
            foreach (Card card in cards)
                this.cards.Add(card);
        }
        public void addCard(Card card)
        {
            this.cards.Add(card);
        }
        public bool addMoney(String cardname, float Amount)
        {
            bool check = false;
            foreach (Card c in this.cards)
            {
                if (c.Name == cardname)
                {
                    c.addMoney(Amount);
                    check = true;
                    break;
                }
            }
            if (check == false)
            {
                Console.WriteLine("Wrong card!");
            }
            return check;
        }

        public List<Card> GetValidCards(DateTime currentDay)
        {
            List<Card> validCard = new List<Card>();
            foreach (Card card in cards)
            {
                if (DateTime.Compare(currentDay, card.ExpirationDate) < 0)
                    validCard.Add(card);
            }
            return validCard;
        }

        public Dictionary<Card, Tuple<float, float>> GetCardsCost(float totalvalue, DateTime currentDate)
        {
            Dictionary<Card, Tuple<float, float>> D = new Dictionary<Card, Tuple<float, float>>();
            List<Card> validCards = GetValidCards(currentDate);
            foreach (Card card in validCards)
            {
                D.Add(card, GetCardCost(card, totalvalue, validCards.ToList()));
            }
            return D;
        }

        public Card GetSmallestFee(List<Card> cards)
        {
            float fee = 1.1f;
            Card good = null;
            foreach (Card card in cards)
            {
                if (card.Fee < fee)
                {
                    good = card;
                    fee = card.Fee;
                }
            }
            return good;
        }

        public Card GetCard(String Name)
        {
            foreach (Card card in this.cards)
            {
                if (card.Name == Name)
                {
                    return card;
                }
            }
            return null;
        }

        public Tuple<float, float> GetCardCost(Card card, float cost_of_product, List<Card> validCards)
        {
            float fee;
            if (card.AvailableAmount >= cost_of_product)
            {
                fee = cost_of_product * card.Fee;
            }
            else
            {
                validCards.Remove(card);
                fee = cost_of_product * card.Fee;
                float tempamount = card.AvailableAmount;
                do
                {
                    Card c = GetSmallestFee(validCards);
                    float target = cost_of_product - tempamount;
                    float transferfee = target * c.Fee;
                    if (c.AvailableAmount >= target + transferfee)
                    {
                        tempamount = tempamount + target;
                    }
                    else
                    {
                        transferfee = c.AvailableAmount * c.Fee;
                        tempamount += c.AvailableAmount - transferfee;
                        validCards.Remove(c);
                    }
                    fee = fee + transferfee;
                } while (tempamount != cost_of_product);
            }
            float TVA = (cost_of_product - fee) * 19 / 100;
            return new Tuple<float, float>(TVA, fee);
        }
    }

    class Card
    {
        public String Name { get; set; }
        public float Fee { get; set; }
        public DateTime ExpirationDate { get; set; }
        public float AvailableAmount { get; set; }

        public Card(string Name, float Fee, DateTime ExpirationDate)
        {
            this.Name = Name;
            this.Fee = Fee;
            this.ExpirationDate = ExpirationDate;
            this.AvailableAmount = 0;
        }

        public void addMoney(float Amount)
        {
            this.AvailableAmount = this.AvailableAmount + Amount;
        }

        public bool Transfer(Card c, float Amount)
        {
            float feecost = this.Fee * Amount;
            if (Amount <= this.AvailableAmount + feecost)
            {
                c.AvailableAmount = c.AvailableAmount + Amount;
                this.AvailableAmount -= this.AvailableAmount + feecost;
                return true;
            }
            else
            {
                Console.WriteLine("You dont have enough funds! ");
                return false;
            }
        }
    }

    class Silver : Card
    {
        public Silver(DateTime ExpirationDate) : base("Silver", 0.2f, ExpirationDate)
        {

        }
    }

    class Gold : Card
    {
        public Gold(DateTime ExpirationDate) : base("Gold", 0.1f, ExpirationDate)
        {

        }
    }

    class Platinum : Card
    {
        public Platinum(DateTime ExpirationDate) : base("Platinum", 0.3f, ExpirationDate)
        {

        }
    }

    class Iridium : Card
    {
        public Iridium(DateTime ExpirationDate) : base("Iridium", 0.2f, ExpirationDate)
        {

        }
    }

    class Bronze : Card
    {
        public Bronze(DateTime ExpirationDate) : base("Bronze", 0.5f, ExpirationDate)
        {

        }
    }

    class Premium : Card
    {
        public Premium(DateTime ExpirationDate) : base("Premium", 0.15f, ExpirationDate)
        {

        }
    }
}
