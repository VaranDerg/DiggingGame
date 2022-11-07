~ ADDITIONS
- Building Info
	~ This tab gives you information on your Buildings, their Cost, and purpose. Treat it as a quick guide to remember
	what your Buildings do. 
- Weather
	~ Cards now change the in-game Weather in different ways. Additionally, every round, it'll shift from Day to Night
	and visa-versa.

~ CHANGES
Mechanics:
	- You can now Build on Gold Pieces. Buildings on Gold are indestructable but cost 1 additional card. Mines will 
	collect Gold Pieces from the Supply.

Balance:
- Thief (1 Grass -> 2 Grass); Dirty Thief (1 Dirt -> 2 Dirt); Master Thief (1 Grass 1 Dirt -> 1 Grass 1 Dirt 1 Stone, Steal
  4 -> Steal 3)
	~ Thief cards, at the moment, are capable of badly stunting the opponent player very early on. Raising the cost for
	Thief cards to be equivalent with their steal amount gives them a more strategic purpose later on into the game flow
	that allows players to commit more to slowing down their opponent. Master Thief's cost was updated according to this
	rules but its current cost should make its activation amount less prevalent.
- Geologist (1 Dirt 2 Stone -> 2 Dirt 1 Stone)
	~ Geologist is a very powerful card if a player is able to play it, but it still only tends to shine once every few
	games. This updated cost should make the card more prevalent in games and would make losing it to Flood less punishing.
- Master Builder (Rework: Your Building Costs are reduced by 1.)
	~ Master builder, before this rework, sat in an awkward position where actually being able to pay its cost and get its
	max reward was rare and debatably not worth the setup necessary. Its cost was kept the same for this rework to reflect
	its long term reward. Playing Master Builder early in a game could provide huge payoff to a player.
- Earthquake (1 Dirt 2 Stone -> 1 Grass 1 Dirt 1 Stone)
	~ Earthquake is a very powerful card, yet tends to get underutilized due to its 2 Stone cost, leaving most buildings on
	Stone untouched for the entire game. This new cost should reflect its thematic purpose and make it more common in games.
- Metal Detector (1 Stone -> 1 Grass)
	~ Metal Detector had a very expensive cost for a possibly detrimental effect. With updated changes to Gold Pieces on the
	board, Metal Detector should become a commonly played card that leads to interesting strategies between players. 
- Planned Gamble (1 Stone -> 1 Dirt)
	~ Planned Gamble is exactly that, a gamble. And spending stone on a card that could rarely give you benefit isn't 
	something that's too readily done. This change should make Planned Gamble still not overly easy to play, but would give
	it a dynamic purpose in the game and control deck cycling more.
- Dam (1 Dirt 1 Stone -> 2 Dirt)
	~ Protecting a building on Dirt is already fairly niche, making Dam's stone cost fairly unjustified. This should allow
	the card to be easier to play.
- Weed Whacker & Dam (Rework: Discard Immediately, Dice = 1)
	~ Weed Whacker & Dam now both happen immediately once a Building matching their type is selected to be damaged. This is
	to allow the cards cycle back into the deck quicker instead of sitting in a player's hand all game. Additionally, the 
	card now simply forces a roll of 1 to increase simplicity.
- Fertilizer (1 Grass 1 Dirt -> 2 Dirt)
	~ Fertilizer, in comparison to Flowers and Compaction, was somewhat difficult to use at most points of the game due to
	the Dirt Supply being fairly empty. Making it similar to other placement cards by making its cost equivalent to its
	placement suit should allow players to use it more often.
- Gold Cards
   ~ Golden Shovel (1 Grass 1 Dirt -> 1 Dirt 1 Stone)
   ~ Regeneration (3 Grass -> 2 Grass 1 Dirt 1 Stone)
   ~ Tornado (3 Grass 1 Dirt -> 3 Grass 1 Stone)
   ~ Transmutation (1 Stone -> 2 Stone)
   ~ Holy Idol (2 Grass 1 Stone -> 1 Grass 1 Dirt 1 Stone)
	~ Certain games of Subterranean tend to have an influx of Gold Cards with a drought of Stone Pieces in the Supply. These
	changes were put in place to encourage more Stone being sent to the Supply, give Stone Mines a further use for late game
	rounds, and allow placing Stone to be a more viable choice. 

Bugs: 
- Fixed an issue where Garden and Flowers wouldn't allow players to place Pieces on the board and softlock the game.
- Fixed an issue where Garden and Flowers would think that there's no open spots on the board where Pieces could be placed.
- Stopped allowing players to move Pawns onto Pieces with newly added Pawns.
- Corrected an issue where the game would say Buildings have taken damage when they've actually taken none.
- Fixed Gold Pieces staying in an undiggable state.
- Fixed an issue where Planned Gamble would just discard one card.
- Fixed jittery behavior with some Card Discard animations.
- Mines no longer collect Pieces after being destroyed.
- Walkway no longer prompts the player to dig a Grass Piece at times. 
- Shovel no longer operates while in a player's hand. 
- Golden Shovel now correctly removes up to 4 Pieces.
- Fixed some issues with incorrect UI layering. 
- Fixed an issue where Dirty Thief wouldn't let you steal 1 Stone.
- Tornado now only lets you select Building Types if any of them are on the board.
- Fixed an issue where Secret Tunnels couldn't be discarded.
- Thunderstorm no longer softlocks the game if no Buildings are made. 
- Regeneration now scores the correct amount of points listed on the card.
- Fixed an issue where players could sometimes damage multiple buildings with one card.