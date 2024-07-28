import { useNavigate } from "react-router-dom";
import { startPlan } from "./api/api";
import Layout from "./components/Layout/Layout";

const App = () => {
  var navigate = useNavigate();

  function generate5DigitID() {
    return Math.floor(10000 + Math.random() * 90000);
  }
  const numericUUID = generate5DigitID();

  const planData = {
    planId: numericUUID,
    createDate: "2024-07-27T00:00:00",
    updateDate: "2024-07-27T00:00:00",
  };

  const start = async () => {
    var plan = await startPlan(planData);
    navigate(`/plan/${plan.planId}`);
  };

  return (
    <Layout>
      <div className="container">
        <div className="text-center mt-4">
          <h3>Start Here</h3>
          <p>Click "start" to begin</p>
          <button className="btn btn-primary" onClick={() => start()}>
            Start
          </button>
        </div>
      </div>
    </Layout>
  );
};

export default App;
