import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import Navigation from './components/Navigation/Navigation';
import Home from './components/Home/Home';
import WeatherForecast from './components/Forecast/Forecast';
import Resume from './components/Resume/Resume';
import { OrdersPage } from './components/Orders';


function App() {
    return (
        <Router>
            <div className="app">
                <Navigation />
                <main className="main-content">
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="/forecast" element={<WeatherForecast />} />
                        <Route path="/resume" element={<Resume />} />
                        <Route path="/orders" element={<OrdersPage />} />
                    </Routes>
                </main>
            </div>
        </Router>
    );
}

export default App;