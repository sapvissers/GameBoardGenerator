    Project:  Project Falcon
    Author:   Sven Vissers
    Date:     2014-09-29

GameBoardGenerator
==================

Main purpose: Generating a random game board.

For a new turn based strategy game I'm creating at the moment I needed a random generated game board. 
I started researching ways to do this and figured out that Voronoi diagrams probably would do the job.

After puzzeling for a while I found a script someone else wrote to generate voronoi diagrams, 
but it wasn't exactly what I was looking for. It only generated a voronoi texture while I needed actual tiles. 
After a lot of modifications this is my final result. 

The result: A board that randomly generates using Voronoi diagrams. 
To make an early start on the strategy game you can select a tile at the edge of the board.
Expend your teratory by clicking adjacent tiles.

You can play the latest webplayer build on my portfolio: 
http://svenvissers.nl/unityplayer.php?category=programming&unity=BoardGenerator
