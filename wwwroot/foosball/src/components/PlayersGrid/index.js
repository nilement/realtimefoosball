import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import PlayerItem from "./playerItem";
import GridList from '@material-ui/core/GridList';
import GridListTile from '@material-ui/core/GridListTile';
import ListSubheader from '@material-ui/core/ListSubheader';

const styles = theme => ({
  root: {
    display: 'flex',
    flexWrap: 'wrap',
    justifyContent: 'space-around',
    overflow: 'hidden',
    backgroundColor: theme.palette.background.paper,
  },
  gridList: {
    width: 100,
    height: 200,
  },
  icon: {
    color: 'rgba(255, 255, 255, 0.54)',
  },
});

function PlayersList(props) {
  var content = (<div></div>);
  if (props.players){
    content = props.players.map((player) => (
      <PlayerItem player={player} key={player.avatarUrl} addFunction={() => props.addFunction(player)} />
    ))
  };

  return (
    <div>
      <GridList cellHeight={50} >
        <GridListTile key="Players" cols={2} style={{ height: 'auto' }}>
          <ListSubheader component="div">Players</ListSubheader>
        </GridListTile>
        {content}
      </GridList>
    </div>
  );
}

export default withStyles(styles)(PlayersList);