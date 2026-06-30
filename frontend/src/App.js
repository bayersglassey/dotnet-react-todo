
import useSWR from 'swr';

import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Container from '@mui/material/Container';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardActions from '@mui/material/CardActions';
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


function TodoItem({item, mutateItems}) {

  // TODO: add item editing capability.

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
    const form = event.target;
    const elems = form.elements;
    const item = {
        title: elems.title.value,
        content: elems.content.value,
    };
    console.log("Adding item!", item);

    // TODO: we should really disable the form or something here, to
    // prevent multiple submissions.
    // But what if the POST times out?.. we don't want to leave the form
    // disabled forever!.. so we should probably also have a timeout
    // on the POST, etc.
    // We don't want to be doing all these things ourselves, though; we
    // should really be using a React form framework, ideally with SWR
    // integration?..
    // I'm not familiar enough with Material UI to know of such a thing
    // off the top of my head, or how to whip one together quickly.
    await postJson(BASE_API_URL + '/items/create', item);
    mutateItems();

    // Clear the form fields
    // (and re-enable it, if we had disabled it before)
    elems.title.value = '';
    elems.content.value = '';
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
          : items.map(item => <TodoItem
            key={item.itemId}
            item={item}
            mutateItems={mutateItems}
          />)
        }
        <AddTodoItem mutateItems={mutateItems} />
      </Container>
    </>
  );
}
