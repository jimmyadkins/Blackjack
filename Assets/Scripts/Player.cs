public class Player
{
    public int balance { get; private set; } = 50;  // Starting balance of $50

    // Method to place a bet, decreasing the balance
    public bool PlaceBet(int amount)
    {
        if (amount <= balance)
        {
            balance -= amount;
            return true;  // Successfully placed the bet
        }
        return false;  // Not enough balance to place the bet
    }

    // Method to add winnings to the balance
    public void AddWinnings(int amount)
    {
        balance += amount;  // Add the winnings to the balance
    }

    // Reset the balance for testing or new game scenarios
    public void ResetBalance()
    {
        balance = 50;  // Reset to initial balance
    }
}
