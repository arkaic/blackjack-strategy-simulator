using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackSimulator {
    public static class Globals {
        public const int ORDERED = 1;
        public const int SHUFFLED = 2;
        public const int H =  1;
        public const int S =  2;
        public const int DH = 3;
        public const int DS = 4;
        public const int P  = 5;
        public const int PH = 6;
        public const int RH = 7;
        public const int K  = 0;
        public const int NEXT_ROUND = 1;
        public const int BREAK = 2;
        public static bool takeInsurance = false;
        public static bool houseStaysOnSoft17 = true;
        public static bool doubleAllowedAfterSplit = true;
        public static bool surrendersAllowed = true;
    }

    public static class Stats {
        public static decimal houseBank = 0.00m;
        public static decimal playerBank = 0.00m;
        public static int wins = 0;
        public static int losses = 0;
        public static int pushes = 0;
        public static int surrenders = 0;
        public static decimal totalBets = 0.00m;

        //Expected value = wager*(win prob) - wager*(lose prob)
        //or get calculated values from somewhere
        //actual value = current (+/-)bankroll divided by total wagers made
        public double ActualValue() {            
            return playerBank / totalBets;
        }
    }

    public class Hand {
        public List<int> cards = new List<int>();
        public decimal bet = 0.00m;
        public int hardVal = 0;
        public int realVal = 0;
        public bool hasAce = false;
        public bool isSplit = false;
        public bool isDoubled = false;
        public bool isHouse = false;
        public void DealCard(int card) {
            cards.Add(card);
            hardVal += card;
            if (hardVal >= 12) {
                realVal = hardVal;
            } else {
                if (!hasAce) {
                    if (card != 1) {
                        realVal = hardVal;
                    } else {
                        hasAce = true;
                        realVal = hardVal + 10;
                    }
                } else {
                    realVal = hardVal + 10;
                }
            }
        }
        public bool IsSoft() {
            bool returnVal = (realVal==hardVal) ? false : true;
            return returnVal;
        }        
    }
    class Program {
        static void Main(string[] args) {
//            Testing();
            Test2();
        }
        static void Testing() {
            Console.WriteLine("Blackjack Simulator!!!!!!!!");
            Console.WriteLine("houseBank = {0:C}", Stats.houseBank);
            Console.WriteLine("playerbank = {0:C}", Stats.houseBank);
            Stats.houseBank *= (decimal)1.5;
            Console.WriteLine("Stat.houseBank *= 1.5 = {0:C}", Stats.houseBank);
            Hand hand = new Hand();
            Console.WriteLine("hand.hardVal = {0}\nhand.realVal = {1}", hand.hardVal, hand.realVal);
            Console.WriteLine("issoft() = {0}", hand.IsSoft());
            Console.WriteLine("test LIST for cards...");
            hand.cards.Add(5);
            hand.cards.Add(2);
            Console.WriteLine("hand.cards = [{0}, {1}]", hand.cards[0], hand.cards[1]);
            PrintCollection(hand.cards);
        }
        static void Test2() {
            // rounds up on blackjack payouts to nearest half (1.00, 1.50, 
            // 2.00, etc);
            decimal payout = Stats.playerBank * (decimal)1.5;
            payout *= (decimal)1.5;
            payout = 5.75m;            
            Console.WriteLine("{0:C} => {1:C}", payout, Math.Ceiling(payout*2)/2);
        }
        public static void PrintCollection<T>(IEnumerable<T> col) {
            foreach (T item in col)
                Console.WriteLine(item); // Replace this with your version of printing
        }
        public static void GameLoop() {
            /* 
               while len(shoe) >= shoesize/5:
                    setup hands, wagers
                    deal cards
                    check for dealer bj
                    check for dealer ace-bj and insurance
                    resolve player blackjacks
                    
                    loop over hands:
                       deal card if hand size = 1
                       check for splitting option
                       loop for dealing cards:
                           decisions, decisions
                       if hand busts or surrenders, do stuff and remove it
                       else, increment hand counter
                    
                    if there are player hands left:
                        deal to house and evaluate
                        pay and take
                    
                    clear hands
            */
        }
        public static void RunSimulation() {
            /* while runs:
                make shoe
                game loop
                runs--
            */
        }
    }
}
