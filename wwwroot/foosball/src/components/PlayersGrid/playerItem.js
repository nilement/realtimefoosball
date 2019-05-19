import React from 'react';
import PropTypes from 'prop-types';
import GridListTile from '@material-ui/core/GridListTile';
import GridListTileBar from '@material-ui/core/GridListTileBar';

function PlayerItem(props){
    return(
        <GridListTile onClick={props.addFunction}>
            <img src={'http://localhost:5000/static/media/user.jpg'} />
            <GridListTileBar
                title={props.player.name}
                subtitle={<span>Record: {props.player.wins}-{props.player.losses}</span>}
            />
        </GridListTile>
    );
}

PlayerItem.propTypes = {
    player: PropTypes.object,
    addAsBlue: PropTypes.func
};

export default PlayerItem;