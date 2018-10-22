[license-image]: https://img.shields.io/npm/l/normalize.css.svg?style=flat
[license-url]: LICENSE

# Checkers [![license][license-image]][license-url]
The application is a classic game of checkers. The game has 2 modes: PvP (network game) and against the AI.

- WPF;
- WPF vector graphics;
- WPF Material Design;
- WCF;
- TCP/IP;
- LINQ to SQL;
- MS SQL DB;
- MVVM;
- IoC;
- Autofac;
- Data Binding;
- Game lobby with chat;
- Client/Server model;
- Autoscale game board.

## Documentation
By selecting a player from the list (LMB) you can invite him to the game. Or by the command from the context menu. All players keep statistics of wins and losses. Statistics are stored in a database. Depending on the number of wins the player in the lobby changes avatar.

Ranks: 
- 10  wins = bronze;
- 50  wins = silver;
- 100 wins = gold;
- 200 wins = Platinum;
- 300 wins = Diamond;
- 400 wins = master;
- 500 wins = grandmaster.

### TODO list:
* Private messages in a separate tab TabControl (in the lobby);
* View statistics (profile) of the player in a separate window;
* Play white against AI;
* Ability to choose the difficulty of AI;
* Notify if opponent leaves the game;
* Rework database.
