win = int(input("Enter Wins: "))
draw = int(input("Enter Draws: "))
lose = int(input("Enter loses: "))
games = win + draw + lose
Elo = 30 * ((win + draw / 2)/ games -0.5)
Elo = round(Elo, 3)
if Elo > 0:
    Elo = "+" + str(Elo)
print("W/D/L: " + str(win) + "/" + str(draw) + "/" + str(lose) + " Games: " + str(games) + " Elo: " + str(Elo))