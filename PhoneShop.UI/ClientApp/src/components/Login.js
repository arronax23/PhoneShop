import React, {useState} from 'react'
import TextField from '@material-ui/core/TextField'
import MenuItem from '@material-ui/core/MenuItem'
import Button from '@material-ui/core/Button'
import { makeStyles } from '@material-ui/styles';
import { useHistory } from 'react-router';
import { Typography } from '@material-ui/core';

const useStyles = makeStyles({
    root: {
        width: '75%',
        marginBottom: 16,
        marginTop: 16
    },
    errorMessage: {
        color: 'red'
    },
    role: {
        color: 'green'
    }
});

function Login() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState();
    const [role, setRole] = useState('');
    const classes = useStyles();
    const history = useHistory();

    const onSubmit = (e) => {
        e.preventDefault();
        fetch('api/Login',{
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({username, password})
        })
        .then(resp => {
            console.log(resp);
            if (resp.ok){
                // history.push(`/succesfulregistration/${username}`)
                console.log('ok');
                return resp.text()
                .then(data => {
                    console.log(data);
                    setRole(data)
                })
            }
            else{
                console.log('entered else');
                return resp.json()        
                .then(data => {
                    console.log(data);
                    setError(data.detail)
                })
            }
        })
        .catch(err =>console.log(err.message));
    }

    return (
        <div>
            <form onSubmit={onSubmit} className="form">
                <div>
                <TextField className={classes.root} variant="outlined" type="text" label="Username" value={username} onChange={(e)=> setUsername(e.target.value)} />
                </div>
                <div>
                 <TextField className={classes.root} variant="outlined" type="password" label="Password" value={password} onChange={(e)=> setPassword(e.target.value)} />
                </div>
                <div>
                <div>
                    {error && <Typography className={classes.errorMessage}>{error}</Typography>}
                </div>
                <div>
                    {role && <Typography className={classes.role}>{role}</Typography>}
                </div>
                <Button type="submit" variant="contained" color="primary">Login</Button>
                </div>
            </form>
        </div>
    )
}

export default Login
