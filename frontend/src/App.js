
import useSWR from 'swr';

import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Container from '@mui/material/Container';
import Box from '@mui/material/Box';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import CssBaseline from '@mui/material/CssBaseline';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import ExpandMore from '@mui/icons-material/ExpandMore';

const BASE_API_URL = process.env.BASE_API_URL || 'http://localhost:3001';


const api_fetcher = url => fetch(url).then(res => res.json()).then(data => {
    // This is the "fetcher" function for use with SWR.
    // That is, SWR maintains a cache of API calls, and when it needs to
    // actually make a call, this function is what it calls.
    //console.log("Got data for: " + url, data);
    return data;
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


function AddTodoItem() {
  return (
    <Accordion />
  );
}


export default function App() {

  // NOTE: SWR can do pagination quite easily, see: https://swr.vercel.app/docs/pagination
  // And Material UI has a Pagination component: https://mui.com/material-ui/react-pagination/
  // ...however, our API doesn't currently support pagination!
  const { data: items, error, isLoading } = useSWR(
    BASE_API_URL + '/items/list', api_fetcher);

  return (
    <>
      <CssBaseline />
      <Container component="main" maxWidth="xs">
        <Box sx={{ flexGrow: 1 }}>
          <AppBar position="static">
            <Toolbar>
              <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                News
              </Typography>
            </Toolbar>
          </AppBar>
        </Box>
        {
          error? <Typography>Error loading TODO items!</Typography>
          : isLoading? <Typography>Loading TODO items...</Typography>
          : !items || !items.length? <Typography>No items yet.</Typography>
          : items.map(item => <TodoItem key={item.itemId} item={item} />)
        }
        <AddTodoItem />
      </Container>
    </>
  );
}
