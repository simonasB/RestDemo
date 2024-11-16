import {useState, useEffect} from 'react';
import axios from 'axios';

const App = () => {
    const [topics, setTopics] = useState([]);

    useEffect(() => {
        const loadTopics = async () => {
            //const response = await axios.get('https://sea-lion-app-vayj4.ondigitalocean.app/api/topics');
            const response = await axios.get('http://localhost:5106/api/topics');
            console.log(response.data.resource);
            setTopics(response.data.resource);
        };

        loadTopics();
    }, []);

    return (
        <>
        {topics.map((topic, i) => (
            <p key={i}>{topic.resource.title} {topic.resource.description}</p>
        ))}
        </>
    )
}

export default App;