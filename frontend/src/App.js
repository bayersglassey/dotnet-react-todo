
import useSWR, { useSWRConfig } from 'swr';

import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';
import AccordionDetails from '@mui/material/AccordionDetails';
import Typography from '@mui/material/Typography';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Button from '@mui/material/Button';


// TODO: make this URL configurable.
const BASE_API_URL = 'http://localhost:3001';


const fetcher = url => fetch(url).then(res => res.json()).then(data => {
    // This is the "fetcher" function for use with SWR.
    // I'm logging stuff at the moment to debug SWR and its cache...
    console.log("Got data for: " + url, data);
    return data;
});


function TodoItem({item}) {

  // TODO: add item editing capability.
  // See: https://swr.vercel.app/docs/mutation
  // ...oh wait, we're using SWR 2.0, so: https://swr.vercel.app/blog/swr-v2

  return (
    <Accordion>
      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
        <Typography component="span">Item {item.itemId}: {item.title}</Typography>
      </AccordionSummary>
      <AccordionDetails>
        <Typography>{item.content}</Typography>
        <Button>Edit</Button>
      </AccordionDetails>
    </Accordion>
  );
}


export default function App() {

  // NOTE: SWR can do pagination quite easily, see: https://swr.vercel.app/docs/pagination
  // And Material UI has a Pagination component: https://mui.com/material-ui/react-pagination/
  // ...however, our API doesn't currently support pagination!
  const { data: items, error, isLoading } = useSWR(BASE_API_URL + '/items/list', fetcher);

  console.log("App with items", items);
  const { cache } = useSWRConfig();
  console.log("Cache", cache);

  return (
    <div className="App">
      {
        error? <Typography>Error loading TODO items!</Typography>
        : isLoading? <Typography>Loading TODO items...</Typography>
        : !items || !items.length? <Typography>No items yet.</Typography>
        : items.map(item => <TodoItem key={item.itemId} item={item} />)
      }
    </div>
  );
}
