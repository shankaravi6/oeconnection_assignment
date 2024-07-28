import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({ procedure, users, selectedUsers, handleAssignUserToProcedure }) => {
  const [selectedOptions, setSelectedOptions] = useState([]);

  useEffect(() => {
    if (selectedUsers) {
      setSelectedOptions(selectedUsers);
    }
  }, [selectedUsers]);

  const handleChange = (selectedOptions) => {
    setSelectedOptions(selectedOptions);
    handleAssignUserToProcedure(selectedOptions, procedure.procedureId);
  };

  return (
    <div className="py-2">
      <div>{procedure.procedureTitle}</div>
      <ReactSelect
        className="mt-2"
        placeholder="Select User to Assign"
        isMulti={true}
        options={users}
        value={selectedOptions}
        onChange={handleChange}
      />
    </div>
  );
};

export default PlanProcedureItem;
