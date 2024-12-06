using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class Program
    {
        enum Shape
        {
            Clover, Diamond, Heart, Spade
        }

        struct Card
        {
            public Shape shape;
            public string number;
            public Card(Shape shape, string number)
            {
                this.shape = shape;
                this.number = number;
            }
        }
        static void Main(string[] args)
        {
            #region 선언부분
            Random rnd = new Random();

            int input = 0; //입력값
            int deckCount = 0; //덱 인덱스 카운팅
            int playerCount = 0; //플레이어 손패 인덱스
            int dealerCount = 0; //딜러 손패 인덱스
            int playerPoint = 1000; //플레이어 돈
            int dealerPoint = 1000; //딜러 돈
            int playerSum = 0; //플레이어 카드의 합
            int dealerSum = 0; //딜러 카드의 합
            int bet = 0; //입력한 돈
            int stake = 0; //판돈
            int burstChecker = 0; //0 버스트 없음 | 1 딜러 버스트 | 2 플레이어 버스트
            int gameCondition = 0; //0 스탠드 아님 | 1 일반 스탠드 | 2 더블 다운 | 3 서렌더 | 4 버스트 | 5 딜러 블랙잭 | 6 플레이어 블랙잭 | 7 쌍방 블랙잭

            Card[] deck = new Card[52]; //덱
            Card[] playerHand = new Card[12]; //플레이어 손패 //손패가 가장 많을 수 있는 경우 1 1 1 1 2 2 2 2 3 3 3 3 = 12개
            Card[] dealerHand = new Card[12]; //딜러 손패
            #endregion

            Console.SetWindowSize(116, 30);
            MakeCard(deck);
            foreach (Card card in deck)
            {
                Console.WriteLine(card.shape.ToString() + card.number);
            }
            Shuffle(deck, rnd);
            Console.WriteLine("\n--------------------------------\n");
            foreach (Card card in deck)
            {
                Console.WriteLine(card.shape.ToString() + card.number);
            }

            while (true)
            {
            StartScreen();
            int.TryParse(Console.ReadLine(), out input);
            switch (input)
            {
                case 1:
                    while (playerPoint >= 100 && dealerPoint >= 100)
                    {

                        gameCondition = 0;
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("플레이어의 포인트 : " + playerPoint + " 딜러의 포인트 : " + dealerPoint + "\n");

                            if (playerPoint <= dealerPoint)
                            {
                                Console.WriteLine($"배팅할 금액을 입력하세요(100~{playerPoint}).");
                                int.TryParse(Console.ReadLine(), out bet);
                                if (bet >= 100 && bet <= playerPoint)
                                {
                                    stake = bet * 2;
                                    break;
                                }
                                continue;
                            }
                            else
                            {
                                Console.WriteLine($"배팅할 금액을 입력하세요(100~{dealerPoint}).");
                                int.TryParse(Console.ReadLine(), out bet);
                                if (bet >= 100 && bet <= dealerPoint)
                                {
                                    stake = bet * 2;
                                    break;
                                }
                                continue;
                            }
                        }
                        playerPoint -= bet;
                        dealerPoint -= bet;

                        Shuffle(deck, rnd);
                        deckCount = 0;
                        playerCount = 0;
                        dealerCount = 0;
                        burstChecker = 0;

                        while (gameCondition != 1 && gameCondition != 2 && gameCondition != 3 && gameCondition != 4)
                        {
                            if (deckCount == 0)
                            {
                                Hit(deck, playerHand, dealerHand, ref deckCount, ref playerCount, ref dealerCount, false);
                                Hit(deck, playerHand, dealerHand, ref deckCount, ref playerCount, ref dealerCount, false);
                                playerSum = CountHand(playerHand, playerCount);
                                dealerSum = CountHand(dealerHand, dealerCount);
                                if (playerSum == 21 && dealerSum == 21)
                                {
                                    gameCondition = 7;
                                    break;
                                }
                                else if (playerSum == 21)
                                {
                                    gameCondition = 6;
                                    break;
                                }
                                else if (dealerSum == 21)
                                {
                                    gameCondition = 5;
                                    break;
                                }
                                continue;
                            }
                            Console.Clear();
                            Console.WriteLine("판돈: " + stake);
                            Console.WriteLine("\n----------------------------------------------\n");
                            ShowHands(playerHand, dealerHand, playerCount, dealerCount, true);
                            Console.WriteLine("\n----------------------------------------------\n");
                            if (gameCondition != 2)
                            {
                                Console.WriteLine("행동을 선택하세요 ");
                                Console.WriteLine("1. Hit 2. Stand 3. Double Down 4. Surrender");
                            }

                            int.TryParse(Console.ReadLine(), out input);
                            switch (input)
                            {
                                case 1:
                                    Hit(deck, playerHand, dealerHand, ref deckCount, ref playerCount, ref dealerCount, false);
                                    break;
                                case 2:
                                    while (CountHand(dealerHand, dealerCount) < 17)
                                    {
                                        Hit(deck, playerHand, dealerHand, ref deckCount, ref playerCount, ref dealerCount, true);
                                    }
                                    gameCondition = 1;
                                    break;
                                case 3:
                                    gameCondition = 2;
                                    Hit(deck, playerHand, dealerHand, ref deckCount, ref playerCount, ref dealerCount, false);
                                    if (CountHand(playerHand, playerCount) > 21)
                                    {
                                        break;
                                    }
                                    while (CountHand(dealerHand, dealerCount) < 17)
                                    {
                                        Hit(deck, playerHand, dealerHand, ref deckCount, ref playerCount, ref dealerCount, true);
                                    }
                                    break;
                                case 4:
                                    gameCondition = 3;
                                    break;
                                default:
                                    continue;
                            }

                            burstChecker = CheckBurst(playerHand, dealerHand, playerCount, dealerCount);

                            if (burstChecker != 0 && gameCondition != 2)
                            {
                                gameCondition = 4;
                            }
                        }

                            Console.Clear();
                        switch (gameCondition) //0 스탠드 아님 | 1 일반 스탠드 | 2 더블 다운 | 3 서렌더 | 4 버스트 | 5 딜러 블랙잭 | 6 플레이어 블랙잭 | 7 쌍방 블랙잭
                        {
                            case 1:
                                Console.Write("스탠드 결과 ");
                                playerSum = CountHand(playerHand, playerCount);
                                dealerSum = CountHand(dealerHand, dealerCount);
                                if (playerSum == dealerSum)
                                {
                                    playerPoint += bet;
                                    dealerPoint += bet;
                                    Console.Write("무승부로 ");
                                }
                                else if (playerSum > dealerSum)
                                {
                                    playerPoint += stake;
                                    Console.Write("플레이어의 승리로 ");
                                }
                                else
                                {
                                    dealerPoint += stake;
                                    Console.Write("딜러의 승리로 ");
                                }
                                break;
                            case 2:
                                Console.Write("더블 다운 결과 ");
                                playerSum = CountHand(playerHand, playerCount);
                                dealerSum = CountHand(dealerHand, dealerCount);
                                if (burstChecker == 2)
                                {
                                    dealerPoint += stake + bet;
                                    playerPoint -= bet;
                                    Console.Write("플레이어의 버스트로 ");
                                }
                                else if (burstChecker == 1)
                                {
                                    playerPoint += stake + bet;
                                    dealerPoint -= bet;
                                    Console.Write("딜러의 버스트로 ");
                                }
                                else if (playerSum == dealerSum)
                                {
                                    playerPoint += bet;
                                    dealerPoint += bet;
                                    Console.Write("무승부로 ");
                                }
                                else if (playerSum > dealerSum)
                                {
                                    playerPoint += stake + bet;
                                    dealerPoint -= bet;
                                    Console.Write("플레이어의 승리로 ");
                                }
                                else
                                {
                                    dealerPoint += stake + bet;
                                    playerPoint -= bet;
                                    Console.Write("딜러의 승리로 ");
                                }
                                break;
                            case 3:
                                dealerPoint += (int)(bet * 1.5);
                                playerPoint += (int)(bet * 0.5);
                                Console.Write("플레이어의 항복으로 ");
                                break;
                            case 4:
                                if (burstChecker == 2)
                                {
                                    dealerPoint += stake;
                                    Console.Write("플레이어의 버스트로 ");
                                }
                                else if (burstChecker == 1)
                                {
                                    playerPoint += stake;
                                    Console.Write("딜러의 버스트로 ");
                                }

                                break;
                            case 5:
                                dealerPoint += stake;
                                Console.Write("딜러의 블랙잭으로 ");
                                break;
                            case 6:
                                playerPoint += stake;
                                Console.Write("플레이어의 블랙잭으로 ");
                                break;
                            case 7:
                                playerPoint += bet;
                                dealerPoint += bet;
                                Console.Write("양측 모두 블랙잭으로 무승부로 ");
                                break;
                        }

                        Console.WriteLine("게임이 종료되었습니다.\n");
                        Console.Write("종료 시점 ");
                        ShowHands(playerHand, dealerHand, playerCount, dealerCount, false);
                        Console.WriteLine("\n확인하려면 아무 키나 누르세요");
                        Console.ReadKey();
                    }
                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("--------------------Rules--------------------");
                    Console.WriteLine("패의 숫자를 합쳐 21을 넘지 않는 선에서 가깝게 만들면 이기는 게임입니다. \n2~10의 숫자는 숫자 그대로, J,Q,K는 10, A는 1 또는 11로 셉니다.\n" +
                        "시작하면 배팅 금액을 정한 후 딜러와 플레이어 모두 카드를 2장씩 받고 플레이어는 딜러의 첫 카드만 확인할 수 있습니다.\n" +
                        "이 단계에서 21을 완성하면 블랙잭이 되어 자동으로 승리합니다.\n" +
                        "첫 카드를 확인하고 플레이어는 Hit, Stand, Double Down, Surrender의 4가지 행동이 가능합니다.\n\n" +
                        "Hit은 서로 카드를 한장씩 뽑습니다. 이 때 카드의 합이 21이 넘어간다면 Burst가 되어 자동으로 패배합니다.\n" +
                        "Stand는 카드를 뽑지 않고 차례를 마쳐 서로의 패를 확인해 카드의 합이 더 큰 쪽이 승리합니다.\n" +
                        "Double Down은 돈을 두배로 걸고 이번 게임동안 카드를 한 장만 더 받습니다. 여기서 합이 21이 넘는다면 Burst처리됩니다.\n" +
                        "Surrender는 이번 게임을 포기하고 배팅의 반을 돌려받습니다.\n\n" +
                        "딜러는 플레이어의 행동과 상관없이 손패의 합이 17이 넘을때까지 카드를 뽑습니다.\n\n" +
                        "아무 키나 눌러서 시작 화면으로 돌아갑니다.");
                    Console.ReadKey();
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("종료되었습니다");
                    Environment.Exit(0); //콘솔 종료
                    break;
            }

                Console.Clear();
                if (playerPoint < 100)
                {
                    Console.WriteLine("플레이어의 포인트가 최소 배팅 금액 이하가 되어 딜러가 승리했습니다.");
                    break;
                }
                else if (dealerPoint < 100)
                {
                    Console.WriteLine("딜러의 포인트가 최소 배팅 금액 이하가 되어 플레이어가 승리했습니다.");
                    break;
                }
            }

        }

        static void StartScreen()
        {
            Console.Clear();
            //Console.SetCursorPosition(9, 10);
            Console.WriteLine(
                " _____                                                   _____ \r\n" +
                "( ___ )-------------------------------------------------( ___ )\r\n" +
                " |   |                                                   |   | \r\n" +
                " |   |  ____  _            _          _            _     |   | \r\n" +
                " |   | | __ )| | __ _  ___| | __     | | __ _  ___| | __ |   | \r\n" +
                " |   | |  _ \\| |/ _` |/ __| |/ /  _  | |/ _` |/ __| |/ / |   | \r\n" +
                " |   | | |_) | | (_| | (__|   <  | |_| | (_| | (__|   <  |   | \r\n" +
                " |   | |____/|_|\\__,_|\\___|_|\\_\\  \\___/ \\__,_|\\___|_|\\_\\ |   | \r\n" +
                " |___|                                                   |___| \r\n" +
                "(_____)-------------------------------------------------(_____)");
            Console.WriteLine("\t\t    1. Start 2. Rule 3. Quit");
        }
        static void MakeCard(Card[] deck)
        {
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    switch (j)
                    {
                        case 0:
                            deck[count] = new Card((Shape)i, "A");
                            break;
                        case 10:
                            deck[count] = new Card((Shape)i, "J");
                            break;
                        case 11:
                            deck[count] = new Card((Shape)i, "Q");
                            break;
                        case 12:
                            deck[count] = new Card((Shape)i, "K");
                            break;
                        default:
                            deck[count] = new Card((Shape)i, (j + 1).ToString());
                            break;
                    }
                    count++;
                }
            }
        }

        static void Shuffle(Card[] deck, Random rnd) //Fisher-Yates shuffle 알고리즘
        {
            Card temp;
            int rndNum = 0;
            for (int i = 0; i < deck.Length; i++)
            {
                rndNum = rnd.Next(0, deck.Length);
                temp = deck[i];
                deck[i] = deck[rndNum];
                deck[rndNum] = temp;
            }
        }

        static void ShowHands(Card[] player, Card[] dealer, int playerCount, int dealerCount, bool hideDealer)
        {
            Console.Write("딜러의 손패: ");
            if (hideDealer)
            {
                for (int i = 0; i < dealerCount; i++)
                {
                    if (i == 0)
                    {
                        switch (dealer[i].shape)
                        {
                            case Shape.Clover:
                                Console.Write("♣");
                                break;
                            case Shape.Diamond:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("◆");
                                break;
                            case Shape.Heart:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("♥");
                                break;
                            case Shape.Spade:
                                Console.Write("♠");
                                break;
                        }
                        Console.Write(dealer[i].number);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("?");
                    }
                    Console.Write(" | ");
                }
            }
            else
            {
                for (int i = 0; i < dealerCount; i++)
                {
                    switch (dealer[i].shape)
                    {
                        case Shape.Clover:
                            Console.Write("♣");
                            break;
                        case Shape.Diamond:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("◆");
                            break;
                        case Shape.Heart:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("♥");
                            break;
                        case Shape.Spade:
                            Console.Write("♠");
                            break;
                    }
                    Console.Write(dealer[i].number);
                    Console.ResetColor();
                    Console.Write(" | ");
                }
            }
            Console.WriteLine("\n");
            Console.Write("플레이어의 손패: ");
            for (int i = 0; i < playerCount; i++)
            {
                switch (player[i].shape)
                {
                    case Shape.Clover:
                        Console.Write("♣");
                        break;
                    case Shape.Diamond:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("◆");
                        break;
                    case Shape.Heart:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("♥");
                        break;
                    case Shape.Spade:
                        Console.Write("♠");
                        break;
                }
                Console.Write(player[i].number);
                Console.ResetColor();
                Console.Write(" | ");
            }

        }

        static int CountHand(Card[] hand, int count)
        {
            int sum = 0;
            int aCount = 0;

            for (int i = 0; i < count; i++)
            {
                switch (hand[i].number)
                {
                    case "A":
                        aCount++;
                        sum += 11;
                        break;
                    case "J":
                    case "Q":
                    case "K":
                        sum += 10;
                        break;
                    default:
                        sum += int.Parse(hand[i].number);
                        break;
                }
            }

            for (int i = 0; i < aCount; i++)
            {
                if (sum > 21)
                {
                    sum -= 10;
                }
            }
            return sum;
        }

        static int CheckBurst(Card[] player, Card[] dealer, int playerCount, int dealerCount) //딜러 버스트 = 1 | 플레이어 버스트 = 2 | 버스트 없음 = 0
        {
            int playerSum = CountHand(player, playerCount);
            int dealerSum = CountHand(dealer, dealerCount);


            if (dealerSum > 21)
            {
                return 1;
            }
            else if (playerSum > 21)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        static void Hit(Card[] deck, Card[] player, Card[] dealer, ref int deckCount, ref int playerCount, ref int dealerCount, bool playerStand)
        {
            if (CountHand(dealer, dealerCount) < 17)
            {
                dealer[dealerCount] = deck[deckCount];
                dealerCount++;
                deckCount++;
            }
            if (!playerStand)
            {
                player[playerCount] = deck[deckCount];
                playerCount++;
                deckCount++;
            }
        }
    }
}
