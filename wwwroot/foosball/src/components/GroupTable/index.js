import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';

const styles = theme => ({
    root: {
      width: '100%',
      marginTop: theme.spacing.unit * 3,
      overflowX: 'auto',
    },
    table: {
      minWidth: 700,
    },
  });

const advanceToPlayoffs = () => {

}

const GroupTable = ({ classes, group, advance }) => (
    <Paper className={classes.root}>
    <Table className={classes.table}>
      <TableHead>
        <TableRow>
          <TableCell align="right">Position</TableCell>
          <TableCell align="right">Name</TableCell>
          <TableCell align="right">Wins</TableCell>
          <TableCell align="right">Loses</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {group.map((player, index) => (
          <TableRow key={index}>
            {/* <TableCell component="th" scope="row">
              ass
            </TableCell> */}
            <TableCell align="right">{index+1}</TableCell>
            <TableCell align="right">{player.player.name}</TableCell>
            <TableCell align="right">{player.wins}</TableCell>
            <TableCell align="right">{player.losses}</TableCell>
            <TableCell align="right"><button onClick={() => advance(player.id)}>Advance to playoffs</button></TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  </Paper>
);

export default withStyles(styles)(GroupTable);