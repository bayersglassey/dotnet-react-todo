
import useSWR from 'swr';

import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Container from '@mui/material/Container';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardActions from '@mui/material/CardActions';
import Box from '@mui/material/Box';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import CssBaseline from '@mui/material/CssBaseline';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import ExpandMore from '@mui/icons-material/ExpandMore';

const BASE_API_URL = process.env.BASE_API_URL || 'http://localhost:3001';


const fetcher = url => fetch(url).then(res => res.json()).then(data => {
    // This is the "fetcher" function for use with SWR.
    // That is, SWR maintains a cache of API calls, and when it needs to
    // actually make a call, this function is what it calls.
    console.log("Fetched data: " + url, data);
    return data;
});


const postJson = (url, data) => fetch(url, {
  method: 'POST',
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json',
  },
  body: JSON.stringify(data),
});


function TodoItem({item}) {

  // TODO: add item editing capability.
  // See: https://swr.vercel.app/docs/mutation
  // ...oh wait, we're using SWR 2.0, so: https://swr.vercel.app/blog/swr-v2

  return (
    <Accordion>
      <AccordionSummary expandIcon={<ExpandMore />}>
        <Typography component="span">Item {item.itemId}: {item.title}</Typography>
      </AccordionSummary>
      <AccordionDetails>
        <Typography>{item.content}</Typography>
        <Button>Edit</Button>
      </AccordionDetails>
    </Accordion>
  );
}


function AddTodoItem({mutateItems}) {

  async function doAdd(event) {
    event.preventDefault();
    const elems = event.target.elements;
    const item = {
        title: elems.title.value,
        content: elems.content.value,
    };
    console.log("Adding item!", item);
    await postJson(BASE_API_URL + '/items/create', item);
    mutateItems();
  }

  return (
    <Card>
      <form onSubmit={doAdd}>
        <CardContent>
          <TextField
            variant="outlined"
            margin="normal"
            required
            fullWidth
            label="Title"
            name="title"
          />
          <TextField
            multiline
            minRows={4}
            variant="outlined"
            margin="normal"
            required
            fullWidth
            label="Content"
            name="content"
          />
        </CardContent>
        <CardActions>
          <Button type="submit">Add TODO Item</Button>
        </CardActions>
      </form>
    </Card>
  );
}


export default function App() {

  // NOTE: SWR can do pagination quite easily, see: https://swr.vercel.app/docs/pagination
  // And Material UI has a Pagination component: https://mui.com/material-ui/react-pagination/
  // ...however, our API doesn't currently support pagination!
  const { data: items, error, isLoading, mutate: mutateItems } = useSWR(
    BASE_API_URL + '/items/list', fetcher);

  return (
    <>
      <CssBaseline />
      <Container component="main" maxWidth="xs">
        <AppBar position="static">
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              Radiology TODO
            </Typography>
          </Toolbar>
        </AppBar>
        {
          error? <Typography>Error loading TODO items!</Typography>
          : isLoading? <Typography>Loading TODO items...</Typography>
          : !items || !items.length? <Typography>No items yet.</Typography>
          : items.map(item => <TodoItem key={item.itemId} item={item} />)
        }
        <AddTodoItem mutateItems={mutateItems} />
      </Container>
    </>
  );
}
