import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  addProcedureToPlan,
  addProcedureUsersToPlan,
  getPlanProcedures,
  getPlanProcedureUsers,
  getProcedures,
  getUsers,
} from "../../api/api";
import Layout from '../Layout/Layout';
import ProcedureItem from "./ProcedureItem/ProcedureItem";
import PlanProcedureItem from "./PlanProcedureItem/PlanProcedureItem";

const Plan = () => {
  let { id } = useParams();
  const [procedures, setProcedures] = useState([]);
  const [planProcedures, setPlanProcedures] = useState([]);
  const [users, setUsers] = useState([]);
  const [formattedData, setFormattedData] = useState([]);

  useEffect(() => {
    (async () => {
      const procedures = await getProcedures();
      const planProcedures = await getPlanProcedures(id);
      const planProcedureUsers = await getPlanProcedureUsers(id);
      const users = await getUsers();

      const userOptions = users.map((u) => ({ label: u.name, value: u.userId }));

      const formattedData = planProcedures.map((pp) => {
        const assignedUsers = planProcedureUsers
          .filter((ppu) => ppu.procedureId === pp.procedureId)
          .map((ppu) => ({
            label: ppu.user.name,
            value: ppu.userId,
          }));
        return {
          procedureId: pp.procedureId,
          selectedUsers: assignedUsers,
        };
      });

      setUsers(userOptions);
      setProcedures(procedures);
      setPlanProcedures(planProcedures);
      setFormattedData(formattedData);
    })();
  }, [id]);

  const handleAddProcedureToPlan = async (procedure) => {
    const hasProcedureInPlan = planProcedures.some((p) => p.procedureId === procedure.procedureId);
    if (hasProcedureInPlan) return;

    await addProcedureToPlan(id, procedure.procedureId);
    setPlanProcedures((prevState) => [
      ...prevState,
      {
        planId: id,
        procedureId: procedure.procedureId,
        procedure: {
          procedureId: procedure.procedureId,
          procedureTitle: procedure.procedureTitle,
          createDate: new Date(),
          updateDate: new Date(),
        },
      },
    ]);
  };

  const handleAssignUserToProcedure = async (selectedUsers, procedureId) => {
    const userIds = selectedUsers.map((user) => user.value).join('-');
    const formattedDataEntry = {
      planId: id,
      procedureId: procedureId,
      users: userIds,
      createDate: new Date(),
      updateDate: new Date(),
    };

    setFormattedData((prevData) => {
      const existingIndex = prevData.findIndex((data) => data.procedureId === procedureId);

      if (existingIndex >= 0) {
        const updatedData = [...prevData];
        updatedData[existingIndex] = { ...formattedDataEntry, selectedUsers };
        return updatedData;
      } else {
        return [...prevData, { ...formattedDataEntry, selectedUsers }];
      }
    });

    await addProcedureUsersToPlan([formattedDataEntry]);

    console.log("Formatted Data:", formattedData);
  };

  return (
    <Layout>
      <div className="container pt-4">
        <div className="d-flex justify-content-center">
          <h2>OEC Interview Frontend</h2>
        </div>
        <div className="row mt-4">
          <div className="col">
            <div className="card shadow">
              <h5 className="card-header">Repair Plan</h5>
              <div className="card-body">
                <div className="row">
                  <div className="col">
                    <h4>Procedures</h4>
                    <div>
                      {procedures.map((p) => (
                        <ProcedureItem
                          key={p.procedureId}
                          procedure={p}
                          handleAddProcedureToPlan={handleAddProcedureToPlan}
                          planProcedures={planProcedures}
                        />
                      ))}
                    </div>
                  </div>
                  <div className="col">
                    <h4>Added to Plan</h4>
                    <div>
                      {planProcedures.map((p) => {
                        const formattedItem = formattedData.find((item) => item.procedureId === p.procedure.procedureId);
                        return (
                          <PlanProcedureItem
                            key={p.procedure.procedureId}
                            procedure={p.procedure}
                            users={users}
                            selectedUsers={formattedItem ? formattedItem.selectedUsers : []}
                            handleAssignUserToProcedure={handleAssignUserToProcedure}
                          />
                        );
                      })}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Plan;
