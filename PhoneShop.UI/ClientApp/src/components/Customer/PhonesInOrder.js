import React, {useState, useEffect} from 'react'
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';
import Button from '@material-ui/core/Button';
import { useSelector } from 'react-redux'
import { makeStyles } from '@material-ui/styles'
import { useParams } from 'react-router'

const useStyles = makeStyles({
    tableContainer: {
      width: '90%',
      margin: 'auto',
      marginTop: 10
    }
  })

function PhonesInOrder() {
    const phoneColors = ["White", "Black", "Red","Blue", "Pink"];
    const classes = useStyles();
    const { id } = useParams();
    const [phones, setPhones] = useState([
        {
            brand: '',
            model: '',
            imageUrl: '',
            ram: '',
            memory: '',
            camera: '',
            os: '',
            color: ''
        }
    ]);
    useEffect(() => {
        fetch('api/GetPhonesInOrder?orderId='+id)
        .then(response => response.json())
        .then(data => {
            setPhones(data);
            console.log(data);
        });
    },[]);

    return (
        <TableContainer className={classes.tableContainer} component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell>Image</TableCell>
              <TableCell>Brand</TableCell>
              <TableCell>Model</TableCell>
              {/* <TableCell>RAM [GB]</TableCell>
              <TableCell>Camera [Mpx]</TableCell>
              <TableCell>Memory [GB]</TableCell>
              <TableCell>OS</TableCell> */}
              <TableCell>Color</TableCell>
              <TableCell>Price [$]</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {phones.map((phone) => (
              <TableRow key={phone.phoneId}>
                <TableCell><img className="phone-table-image" src={phone.imageUrl} /></TableCell>
                <TableCell>{phone.brand}</TableCell>
                <TableCell>{phone.model}</TableCell>
                {/* <TableCell>{phone.ram}</TableCell>
                <TableCell>{phone.camera}</TableCell>
                <TableCell>{phone.memory}</TableCell>
                <TableCell>{phone.os}</TableCell> */}
                <TableCell>{phoneColors[phone.color]}</TableCell>
                <TableCell>{phone.price}&nbsp;$</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    )
}

export default PhonesInOrder