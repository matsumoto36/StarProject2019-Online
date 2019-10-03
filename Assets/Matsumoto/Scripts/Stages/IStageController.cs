using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStageController {

	event Action<IStageController> OnGameStart;
	event Action<IStageController> OnGameClear;
	event Action<IStageController> OnGameOver;

	bool CanPause {
		get; set;
	}
}
